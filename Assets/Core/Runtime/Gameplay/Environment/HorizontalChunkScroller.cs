using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Environment
{
    public class HorizontalChunkScroller : MonoBehaviour
    {
        private enum ScrollDir { Left = -1, Right = 1 }

        [SerializeField] private float     baseScrollSpeed  = 4f;
        [SerializeField] private float     speedMultiplier  = 0.5f;

        /// <summary>Runtime override. ParallaxSpeedProvider writes this in Awake.</summary>
        public float SpeedMultiplier { get => speedMultiplier; set => speedMultiplier = value; }

        [Tooltip("Left = world scrolls toward negative X (standard). Right = world scrolls toward positive X.")]
        [SerializeField] private ScrollDir scrollDirection  = ScrollDir.Left;

        [Tooltip("Extra world units past the screen edge before a chunk is recycled. " +
                 "Increase if chunks pop in while still visible.")]
        [SerializeField] private float recycleBuffer = 5f;

        [Tooltip("Prefab assets. ParallaxLayer instantiates them at runtime as children.")]
        [SerializeField] private GameObject[] chunkPrefabs;

        [Tooltip("X offset from this layer's world position where the first chunk's left edge begins. " +
                 "0 = start at the layer's own X. Consistent with how Y and Z are derived from the transform.")]
        [SerializeField] private float startX = 0f;

        [Tooltip("Minimum extra gap between consecutive chunks (world units).")]
        [SerializeField] private float minDistance = 0f;

        [Tooltip("Maximum extra gap between consecutive chunks (world units).")]
        [SerializeField] private float maxDistance = 0f;

        [Tooltip("If true, each recycled slot picks a random prefab from chunkPrefabs instead of cycling in order.")]
        [SerializeField] private bool randomPick = false;

        // _chunkInstances[slot][prefabIdx] — all variants pre-instantiated, only one enabled per slot.
        // Avoids Destroy/Instantiate at runtime when the active prefab changes on recycle.
        private Transform[][] _chunkInstances;
        private float[][]     _variantWidths;   // precomputed per [slot][prefabIdx]

        private Transform[]   _chunks;          // active instance per slot
        private float[]       _chunkWidths;     // width of the currently active instance per slot
        private int[]         _prefabIndices;   // current prefab index per slot

        // Fully qualified: hp55games.Mobile.Core.Gameplay.Camera (Phase 3) is a sibling
        // namespace under Gameplay, so an unqualified "Camera" here would resolve to that
        // namespace instead of UnityEngine.Camera.
        private UnityEngine.Camera _camera;

        // ── console colours ───────────────────────────────────────────────────
        private const string C_WIDTH   = "#FFD700";
        private const string C_RECYCLE = "#FF6B35";
        private const string C_WARN    = "#FF4444";

        private void Awake()
        {
            _camera = UnityEngine.Camera.main;
            if (_camera == null)
                Debug.LogWarning($"[HorizontalChunkScroller] '{name}': Camera.main not found — recycle bound will use fallback.", this);

            // Guard: null or empty array — nothing to do, disable to avoid Update overhead.
            if (chunkPrefabs == null || chunkPrefabs.Length == 0)
            {
                Debug.LogError($"[HorizontalChunkScroller] '{name}': chunkPrefabs is null or empty — component disabled.", this);
                enabled = false;
                return;
            }

            // Guard: null entries inside the array would crash Instantiate.
            for (int i = 0; i < chunkPrefabs.Length; i++)
            {
                if (chunkPrefabs[i] != null) continue;
                Debug.LogError($"[HorizontalChunkScroller] '{name}': chunkPrefabs[{i}] is null — component disabled.", this);
                enabled = false;
                return;
            }

            // Guard: inverted range would make Random.Range always return minDistance.
            if (minDistance > maxDistance)
                (minDistance, maxDistance) = (maxDistance, minDistance);
        }

        private void Start()
        {
            // Awake may have disabled the component on invalid config — bail out early.
            if (!enabled) return;

            int variantCount = chunkPrefabs.Length;

            // ── Dynamic slot count ────────────────────────────────────────────────
            // Instantiate one reference variant to measure the real prefab width
            // before committing to a slot count. This avoids a chicken-and-egg
            // problem: we need the width to compute slots, but slots drive Instantiate.
            GameObject probe      = Instantiate(chunkPrefabs[0], transform);
            float      probeWidth = ComputeChunkWidth(probe.transform);
            Destroy(probe);

            // Fallback: if the prefab has no measurable width, treat it as 1 unit
            // so the calculation still produces a valid slot count.
            if (probeWidth <= 0f)
            {
                Debug.LogWarning($"[HorizontalChunkScroller] '{name}': first prefab variant has zero width — defaulting to 1 unit.", this);
                probeWidth = 1f;
            }

            // Frustum width at this layer's Z depth, same viewport math as ComputeRecycleBound.
            float depth          = (_camera != null)
                ? _camera.WorldToViewportPoint(transform.position).z
                : 0f;
            float frustumWidth   = (_camera != null)
                ? _camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, depth)).x
                - _camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth)).x
                : 20f;   // safe fallback when camera is unavailable

            int slotCount = Mathf.Max(3, Mathf.CeilToInt(frustumWidth / probeWidth) + 2);

            // ── Array allocation ──────────────────────────────────────────────────
            _chunkInstances = new Transform[slotCount][];
            _variantWidths  = new float[slotCount][];
            _chunks         = new Transform[slotCount];
            _chunkWidths    = new float[slotCount];
            _prefabIndices  = new int[slotCount];

            // ── First pass: instantiate all variants for every slot ───────────────
            // Only the variant at index (slot % variantCount) starts enabled.
            // Width must be computed BEFORE deactivating: GetComponentsInChildren
            // with includeInactive=false returns nothing on an inactive root.
            for (int slot = 0; slot < slotCount; slot++)
            {
                _chunkInstances[slot] = new Transform[variantCount];
                _variantWidths[slot]  = new float[variantCount];

                int initialPrefabIdx  = slot % variantCount;
                _prefabIndices[slot]  = initialPrefabIdx;

                for (int v = 0; v < variantCount; v++)
                {
                    GameObject instance      = Instantiate(chunkPrefabs[v], transform);
                    instance.name            = $"{chunkPrefabs[v].name}_slot{slot}";
                    _chunkInstances[slot][v] = instance.transform;
                    _variantWidths[slot][v]  = ComputeChunkWidth(instance.transform);
                    instance.SetActive(v == initialPrefabIdx);
                }

                _chunks[slot]      = _chunkInstances[slot][initialPrefabIdx];
                _chunkWidths[slot] = _variantWidths[slot][initialPrefabIdx];
            }

            // ── Second pass: position slots side-by-side ──────────────────────────
            float cursorX = transform.position.x + startX;
            for (int slot = 0; slot < slotCount; slot++)
            {
                if (_chunks[slot] == null) continue;

                float half      = _chunkWidths[slot] * 0.5f;
                Vector3 slotPos = new Vector3(cursorX + half, transform.position.y, transform.position.z);

                // Position all variants of this slot at the same X so that
                // enabling a different variant never causes a position jump.
                for (int v = 0; v < _chunkInstances[slot].Length; v++)
                    _chunkInstances[slot][v].position = slotPos;

                float gap  = (slot < slotCount - 1) ? Random.Range(minDistance, maxDistance) : 0f;
                cursorX   += _chunkWidths[slot] + gap;
            }
        }

        private void Update()
        {
            if (_chunks == null || _chunks.Length == 0) return;

            int   dir   = (int)scrollDirection;
            float delta = baseScrollSpeed * speedMultiplier * Time.deltaTime;
            float bound = ComputeRecycleBound();

            for (int slot = 0; slot < _chunks.Length; slot++)
            {
                Transform chunk = _chunks[slot];
                if (chunk == null) continue;

                // Translate active instance; inactive variants track the same position
                // so they are never out of place on the next variant swap.
                Vector3 pos = chunk.position;
                pos.x      += dir * delta;

                for (int v = 0; v < _chunkInstances[slot].Length; v++)
                    if (_chunkInstances[slot][v] != null)
                        _chunkInstances[slot][v].position = pos;

                // Recycle check
                bool recycle = scrollDirection == ScrollDir.Left
                    ? pos.x + _chunkWidths[slot] * 0.5f < bound
                    : pos.x - _chunkWidths[slot] * 0.5f > bound;

                if (!recycle) continue;

                // Pick next prefab
                int nextPrefabIdx = randomPick
                    ? Random.Range(0, chunkPrefabs.Length)
                    : (_prefabIndices[slot] + 1) % chunkPrefabs.Length;

                // Swap active variant if needed — no Destroy/Instantiate.
                if (nextPrefabIdx != _prefabIndices[slot])
                {
                    _chunkInstances[slot][_prefabIndices[slot]].gameObject.SetActive(false);
                    _chunkInstances[slot][nextPrefabIdx].gameObject.SetActive(true);
                    _prefabIndices[slot] = nextPrefabIdx;
                    _chunks[slot]        = _chunkInstances[slot][nextPrefabIdx];
                    _chunkWidths[slot]   = _variantWidths[slot][nextPrefabIdx];
                    chunk                = _chunks[slot];
                }

                // Reposition past the extreme chunk in the scroll direction.
                // If no valid extreme is found (all slots null), skip — corrupted state.
                if (!TryGetExtremeChunk(rightmost: scrollDirection == ScrollDir.Left,
                        out float extremeCentreX, out float extremeHalfWidth))
                    continue;

                float gap = Random.Range(minDistance, maxDistance);

                // Guard: if gap is negative enough the recycled chunk's centre would land
                // behind the extreme chunk's centre, inverting the ordering and causing
                // a cascade of mis-placed recycles. Clamp to the minimum value that keeps
                // the recycled centre strictly past the extreme centre.
                float minSafeGap = -(extremeHalfWidth + _chunkWidths[slot] * 0.5f);
                if (gap < minSafeGap)
                    gap = minSafeGap;
                

                float newX = scrollDirection == ScrollDir.Left
                    ? extremeCentreX + extremeHalfWidth + gap + _chunkWidths[slot] * 0.5f
                    : extremeCentreX - extremeHalfWidth - gap - _chunkWidths[slot] * 0.5f;

                Vector3 newPos = pos;
                newPos.x = newX;
                newPos.x += dir * delta;

                // Move all variants together so none are out of place on next swap.
                for (int v = 0; v < _chunkInstances[slot].Length; v++)
                    if (_chunkInstances[slot][v] != null)
                        _chunkInstances[slot][v].position = newPos;

            }
        }

        // ── Recycle bound — derived from camera viewport ──────────────────────
        private float ComputeRecycleBound()
        {
            // Lazy re-fetch: the camera may live in a different scene (e.g. menu loaded
            // additively) and may not be tagged MainCamera yet when Awake runs here.
            if (_camera == null)
                _camera = UnityEngine.Camera.main;

            float speedMargin = baseScrollSpeed * speedMultiplier * Time.deltaTime;

            if (_camera == null)
            {
                float fallback = scrollDirection == ScrollDir.Left ? -20f : 20f;
                return scrollDirection == ScrollDir.Left
                    ? fallback - speedMargin
                    : fallback + speedMargin;
            }

            // WorldToViewportPoint.z gives the real depth to this layer's plane,
            // making the bound correct at any FOV and for orthographic cameras alike.
            float depth = _camera.WorldToViewportPoint(transform.position).z;

            return scrollDirection == ScrollDir.Left
                ? _camera.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth)).x - recycleBuffer - speedMargin
                : _camera.ViewportToWorldPoint(new Vector3(1f, 0.5f, depth)).x + recycleBuffer + speedMargin;
        }

        // ── Extreme chunk helper — replaces four separate O(n) loops ─────────
        // Returns the centre X and half-width of the rightmost (or leftmost) active chunk.
        // Returns false if no valid chunk was found — caller must guard against this.
        private bool TryGetExtremeChunk(bool rightmost, out float centreX, out float halfWidth)
        {
            float extreme = rightmost ? float.MinValue : float.MaxValue;
            int   idx     = -1;

            for (int i = 0; i < _chunks.Length; i++)
            {
                if (_chunks[i] == null) continue;
                float x = _chunks[i].position.x;
                if (rightmost ? x > extreme : x < extreme) { extreme = x; idx = i; }
            }

            if (idx < 0)
            {
                centreX   = 0f;
                halfWidth = 0f;
                return false;
            }

            centreX   = extreme;
            halfWidth = _chunkWidths[idx] * 0.5f;
            return true;
        }

        // ── Width computation ─────────────────────────────────────────────────
        // Priority 1: explicit ChunkWidthOverride component on the chunk root.
        //   Use this for particle-only chunks whose ParticleSystemRenderer
        //   reports zero bounds before the first emission (t=0).
        // Priority 2: aggregate world bounds from all Renderer components.
        // Priority 3: localScale.x when no Renderer is present.
        // Minimum of 0.01 is enforced to prevent zero-width chunks from
        // breaking recycle positioning.
        private static float ComputeChunkWidth(Transform chunk)
        {
            var overrideComp = chunk.GetComponent<ChunkWidthOverride>();
            if (overrideComp != null)
                return overrideComp.Width;

            Renderer[] renderers = chunk.GetComponentsInChildren<Renderer>();

            float width;
            if (renderers.Length == 0)
            {
                width = chunk.localScale.x;
            }
            else
            {
                Bounds aggregate = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                    aggregate.Encapsulate(renderers[i].bounds);
                width = aggregate.size.x;
            }

            if (width <= 0f)
            {
                Debug.LogWarning($"[HorizontalChunkScroller] '{chunk.name}': computed width is {width:F3} — clamped to 0.01. " +
                                 "Check Renderer bounds or localScale.x on the prefab.", chunk);
                width = 0.01f;
            }

            return width;
        }
    }
}
