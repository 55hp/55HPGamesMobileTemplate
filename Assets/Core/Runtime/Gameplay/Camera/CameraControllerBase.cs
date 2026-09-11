using UnityEngine;
using hp55games.Mobile.Core.Juice;

namespace hp55games.Mobile.Core.Gameplay.Camera
{
    /// <summary>
    /// Base follow-camera rig. Smoothing runs in LateUpdate so the camera reacts to the
    /// target's final position for this frame, not a stale one from before the target's
    /// own Update ran. Uses SmoothDamp (not Lerp) because it's frame-rate independent and
    /// critically damped — a raw Lerp re-scaled by deltaTime still overshoots/jitters at
    /// variable frame rates, SmoothDamp doesn't.
    /// </summary>
    public abstract class CameraControllerBase : MonoBehaviour, ICameraController
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private Vector3 _offset = new Vector3(0f, 0f, -10f);

        [SerializeField]
        private float _smoothSpeed = 0.125f;

        [SerializeField]
        [Tooltip("Optional. Shake() delegates here instead of reimplementing shake logic — " +
                 "leave unassigned to make Shake() a no-op on cameras that don't need it.")]
        private CameraShakeFeedback _shakeFeedback;

        private Vector3 _velocity;
        private bool _warnedMissingShakeFeedback;

        public Transform Target => _target;

        public Vector3 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public float SmoothSpeed
        {
            get => _smoothSpeed;
            set => _smoothSpeed = value;
        }

        protected virtual void OnEnable()
        {
            // Avoids a visible swoop from the rig's editor-time position to the target on
            // the very first frame the camera becomes active (scene load / respawn).
            if (_target != null)
                SnapToTarget();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void Shake()
        {
            if (_shakeFeedback == null)
            {
                if (!_warnedMissingShakeFeedback)
                {
                    Debug.LogWarning("[CameraControllerBase] Shake() called with no CameraShakeFeedback assigned — no-op.", this);
                    _warnedMissingShakeFeedback = true;
                }
                return;
            }

            _shakeFeedback.Activate();
        }

        public void SnapToTarget()
        {
            if (_target == null)
                return;

            transform.position = _target.position + _offset;
            _velocity = Vector3.zero;
        }

        protected virtual void LateUpdate()
        {
            if (_target == null)
                return;

            Vector3 desiredPosition = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothSpeed);
        }
    }
}
