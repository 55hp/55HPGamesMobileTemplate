using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Movement
{
    /// <summary>
    /// Base waypoint/path follower. MoveTo is just SetPath with a one-element array — a
    /// single destination is a path of length 1, so both cases share the same arrival and
    /// looping logic instead of two parallel implementations.
    /// </summary>
    public abstract class WaypointMoverBase : MonoBehaviour, IMover
    {
        [SerializeField]
        private float _speed = 3f;

        [SerializeField]
        [Tooltip("Squared internally to compare against sqrMagnitude — avoids a sqrt every frame.")]
        private float arrivalThreshold = 0.1f;

        [SerializeField]
        [Tooltip("On reaching the last waypoint, wrap to index 0 instead of stopping.")]
        private bool loop = false;

        private Vector3[] _waypoints;
        private int _currentIndex;
        private bool _hasArrived = true;

        public bool HasArrived => _hasArrived;

        public event Action OnDestinationReached;

        public void SetPath(Vector3[] waypoints)
        {
            _waypoints = waypoints;
            _currentIndex = 0;
            _hasArrived = waypoints == null || waypoints.Length == 0;
        }

        public void MoveTo(Vector3 destination, float speed)
        {
            _speed = speed;
            SetPath(new[] { destination });
        }

        protected virtual void Update()
        {
            if (_hasArrived || _waypoints == null || _waypoints.Length == 0)
                return;

            Vector3 toWaypoint = _waypoints[_currentIndex] - transform.position;
            if (toWaypoint.sqrMagnitude <= arrivalThreshold * arrivalThreshold)
            {
                AdvanceWaypoint();
                return;
            }

            transform.position += toWaypoint.normalized * (_speed * Time.deltaTime);
        }

        private void AdvanceWaypoint()
        {
            bool isLastWaypoint = _currentIndex >= _waypoints.Length - 1;
            if (!isLastWaypoint)
            {
                _currentIndex++;
                return;
            }

            if (loop)
            {
                _currentIndex = 0;
                return;
            }

            _hasArrived = true;
            OnDestinationReached?.Invoke();
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            if (_waypoints == null || _waypoints.Length < 2)
                return;

            Gizmos.color = Color.yellow;
            for (int i = 0; i < _waypoints.Length - 1; i++)
                Gizmos.DrawLine(_waypoints[i], _waypoints[i + 1]);

            if (loop)
                Gizmos.DrawLine(_waypoints[_waypoints.Length - 1], _waypoints[0]);
        }
#endif
    }
}
