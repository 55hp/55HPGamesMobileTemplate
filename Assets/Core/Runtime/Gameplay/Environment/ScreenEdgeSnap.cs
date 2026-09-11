using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Environment
{
    /// <summary>
    /// On Awake, moves this GameObject just outside the camera frustum on the
    /// specified side. Only the axis perpendicular to the chosen border is modified;
    /// the other axes are left unchanged.
    ///
    /// Use <see cref="offset"/> to push the object further out (positive) or pull
    /// it partially inside the frustum (negative). Set it to the object's half-extent
    /// on the relevant axis to guarantee the object is completely off-screen.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ScreenEdgeSnap : MonoBehaviour
    {
        public enum Border { Left, Right, Top, Bottom }

        [Tooltip("Which frustum edge to snap to.")]
        [SerializeField] private Border border = Border.Left;

        [Tooltip("Additional world-unit offset applied after snapping. " +
                 "Positive = further outside the frustum. " +
                 "Negative = partially inside.")]
        [SerializeField] private float offset = 0f;

        private void Awake()
        {
            UnityEngine.Camera cam = UnityEngine.Camera.main;

            if (cam == null)
            {
                Debug.LogWarning("[ScreenEdgeSnap] Camera.main not found — snapping skipped.", this);
                return;
            }

            // Compute the depth of this object's Z plane relative to the camera.
            // Same technique used in HorizontalChunkScroller.ComputeRecycleBound():
            // WorldToViewportPoint.z gives the correct distance for ViewportToWorldPoint,
            // working for both orthographic and perspective cameras.
            float depth = cam.WorldToViewportPoint(transform.position).z;

            Vector3 pos = transform.position;

            switch (border)
            {
                case Border.Left:
                    pos.x = cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, depth)).x - offset;
                    break;

                case Border.Right:
                    pos.x = cam.ViewportToWorldPoint(new Vector3(1f, 0.5f, depth)).x + offset;
                    break;

                case Border.Top:
                    pos.y = cam.ViewportToWorldPoint(new Vector3(0.5f, 1f, depth)).y + offset;
                    break;

                case Border.Bottom:
                    pos.y = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, depth)).y - offset;
                    break;
            }

            transform.position = pos;
        }
    }
}
