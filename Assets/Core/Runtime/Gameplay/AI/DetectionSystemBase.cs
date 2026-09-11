using System;
using System.Collections;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.AI
{
    /// <summary>
    /// 3D line-of-sight + field-of-view detection, polled on a Coroutine rather than every
    /// Update — detection checks involve a raycast and angle/distance math, too costly to
    /// run per-frame for every AI entity on mobile; checkInterval trades detection latency
    /// for CPU budget. See DetectionSystemBase2D for the Physics2D.Raycast equivalent.
    /// </summary>
    public abstract class DetectionSystemBase : MonoBehaviour, IDetectionSystem
    {
        [SerializeField]
        [Tooltip("Seconds between line-of-sight checks. Lower = more responsive, more CPU.")]
        private float checkInterval = 0.2f;

        [SerializeField]
        private float detectionRadius = 10f;

        [SerializeField]
        [Tooltip("Full cone angle, in degrees, centered on transform.forward.")]
        private float fieldOfViewAngle = 90f;

        [SerializeField]
        [Tooltip("Layers that block line-of-sight (obstacles/walls), not the target itself.")]
        private LayerMask obstructionMask;

        [SerializeField]
        [Tooltip("Seconds of continued non-visibility required before OnTargetLost fires. " +
                  "Absorbs single missed checks (e.g. target briefly behind a thin prop) so " +
                  "detection state doesn't flicker on/off every poll.")]
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

            Vector3 toTarget = target.position - transform.position;
            float distance = toTarget.magnitude;
            if (distance > detectionRadius)
                return false;

            float angle = Vector3.Angle(transform.forward, toTarget);
            if (angle > fieldOfViewAngle * 0.5f)
                return false;

            // A hit on the obstruction mask before reaching the target means something is
            // blocking the view; the target layer itself must not be part of obstructionMask.
            if (Physics.Raycast(transform.position, toTarget.normalized, distance, obstructionMask))
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
