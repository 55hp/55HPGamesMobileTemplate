using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Pooling;

namespace hp55games.Mobile.Core.Gameplay
{
    /// <summary>
    /// Despawns a pooled object (returns it to IObjectPoolService)
    /// when it goes outside the specified 2D bounds.
    /// Works for side scrollers, bullets, obstacles, etc.
    /// </summary>
    [RequireComponent(typeof(PooledObject))]
    public sealed class DespawnWhenOutOfBounds2D : MonoBehaviour
    {
        [Header("World Bounds")]
        public float minX = -10f;
        public float maxX = 10f;
        public float minY = -5f;
        public float maxY = 5f;

        private PooledObject _pooled;
        private IObjectPoolService _pool;

        private void Awake()
        {
            _pooled = GetComponent<PooledObject>();
            ServiceRegistry.TryResolve(out _pool);
        }

        private void Update()
        {
            if (_pool == null)
                return;

            var pos = transform.position;

            if (pos.x < minX || pos.x > maxX ||
                pos.y < minY || pos.y > maxY)
            {
                _pool.Release(_pooled);
            }
        }
    }
}