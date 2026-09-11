using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Environment
{
    [RequireComponent(typeof(HorizontalChunkScroller))]
    [DisallowMultipleComponent]
    public sealed class ParallaxSpeedProvider : MonoBehaviour
    {
        [Header("Reference")]
        [Tooltip("Z del layer 'neutro' — tipicamente il layer Gameplay (Z=-4).")]
        [SerializeField] private float referenceZ = -4f;

        [Header("Camera")]
        // Fully qualified: hp55games.Mobile.Core.Gameplay.Camera (Phase 3) is a sibling
        // namespace under Gameplay, so an unqualified "Camera" here would resolve to that
        // namespace instead of UnityEngine.Camera.
        [SerializeField] private UnityEngine.Camera _camera;

        [Header("Debug")]
        [Tooltip("Se true, non modifica speedMultiplier — lascia i valori serializzati invariati.")]
        [SerializeField] private bool overrideAutoCalculation = true;

        [Tooltip("Logga il multiplier calcolato in Awake.")]
        [SerializeField] private bool logCalculation = true;

        private void Awake()
        {
            if (overrideAutoCalculation) return;

            if (_camera == null) _camera = UnityEngine.Camera.main;
            if (_camera == null)
            {
                Debug.LogWarning("[ParallaxSpeedProvider] Camera.main not found.", this);
                return;
            }

            float cameraZ           = _camera.transform.position.z;
            float referenceDistance = Mathf.Abs(referenceZ - cameraZ);
            float layerDistance     = Mathf.Abs(transform.position.z - cameraZ);

            if (referenceDistance < 0.001f || layerDistance < 0.001f)
            {
                Debug.LogWarning($"[ParallaxSpeedProvider] '{name}': distanza troppo piccola, skip.", this);
                return;
            }

            float multiplier = referenceDistance / layerDistance;

            foreach (var s in GetComponents<HorizontalChunkScroller>())
                s.SpeedMultiplier = multiplier;

            if (logCalculation)
                Debug.Log($"[ParallaxSpeedProvider] '{name}': layerZ={transform.position.z} " +
                          $"cameraZ={cameraZ:F1} refZ={referenceZ} → mult={multiplier:F3}", this);
        }
    }
}
