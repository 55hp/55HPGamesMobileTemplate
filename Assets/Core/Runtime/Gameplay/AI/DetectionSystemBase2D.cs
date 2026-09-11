using System;
using System.Collections;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.AI
{
    /// <summary>
    /// 2D counterpart to DetectionSystemBase — same coroutine-polled, distance/angle/raycast
    /// shape, swapped to Physics2D.Raycast. Kept as a separate file (mirroring how
    /// ContactDamager houses both pipelines in one file) because the two diverge on forward
    /// direction: 2D sprites conventionally face along transform.right, not transform.forward.
    /// </summary>
    public abstract class DetectionSystemBase2D : MonoBehaviour, IDetectionSystem
    {
        [SerializeField]
        [Tooltip("Seconds between line-of-sight checks. Lower = more responsive, more CPU.")]
        private float checkInterval = 0.2f;

        [SerializeField]
        private float detectionRadius = 10f;

        [SerializeField]
        [Tooltip("Full cone angle, in degrees, centered on transform.right (2D facing convention).")]
        private float fieldOfViewAngle = 90f;

        [SerializeField]
        [Tooltip("Layers that block line-of-sight (obstacles/walls), not the target itself.")]
        private LayerMask obstructionMask;

        [SerializeField]
        [Tooltip("Seconds of continued non-visibility required before OnTargetLost fires. " +
                  "Absorbs single missed checks so detection state doesn't flicker on/off every poll.")]
        private float loseTargetGrace = 1f;

        private Coroutine _pollRoutine;
        private bool _hasTarget;
        private float _timeSinceLastSeen;
        private Transform _lastSeenTarget;

        public float DetectionRadius => detectionRadius;
        public float FieldOfViewAngle => fieldOfViewAngle;

        public event Action<Transform> OnTargetDetected;
        public event Action<Transform> OnTargetLost;

        protected virtual void OnEnable()
        {
            _pollRoutine = StartCoroutine(PollRoutine());
        }

        protected virtual void OnDisable()
        {
            if (_pollRoutine != null)
                StopCoroutine(_pollRoutine);
            _pollRoutine = null;
        }

        public bool CanSeeTarget(Transform target)
        {
            if (target == null)
                return false;

            Vector2 origin = transform.position;
            Vector2 toTarget = (Vector2)target.position - origin;
            float distance = toTarget.magnitude;
            if (distance > detectionRadius)
                return false;

            float angle = Vector2.Angle(transform.right, toTarget);
            if (angle > fieldOfViewAngle * 0.5f)
                return false;

            // A hit on the obstruction mask before reaching the target means something is
            // blocking the view; the target layer itself must not be part of obstructionMask.
            if (Physics2D.Raycast(origin, toTarget.normalized, distance, obstructionMask))
                return false;

            return true;
        }

        /// <summary>Who this detector looks for — usually the player. Returns null by default
        /// (no target to check) so subclasses that haven't wired one up fail safe, not loudly.</summary>
        protected virtual Transform ResolveCurrentTarget() => null;

        private IEnumerator PollRoutine()
        {
            var wait = new WaitForSeconds(checkInterval);
            while (true)
            {
                Evaluate();
                yield return wait;
            }
        }

        private void Evaluate()
        {
            Transform target = ResolveCurrentTarget();
            bool seesTarget = target != null && CanSeeTarget(target);

            if (seesTarget)
            {
                _timeSinceLastSeen = 0f;
                _lastSeenTarget = target;
                if (!_hasTarget)
                {
                    _hasTarget = true;
                    OnTargetDetected?.Invoke(target);
                }
                return;
            }

            if (!_hasTarget)
                return;

            _timeSinceLastSeen += checkInterval;
            if (_timeSinceLastSeen >= loseTargetGrace)
            {
                _hasTarget = false;
                OnTargetLost?.Invoke(_lastSeenTarget);
            }
        }
    }
}
