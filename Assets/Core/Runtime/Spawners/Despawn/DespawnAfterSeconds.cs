using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Pooling;

namespace hp55games.Mobile.Core.Gameplay
{
    /// <summary>
    /// Returns a pooled object to the ObjectPoolService after a certain amount of seconds.
    /// Useful for visual effects, timed obstacles, temporary objects, etc.
    /// </summary>
    [RequireComponent(typeof(PooledObject))]
    public sealed class DespawnAfterSeconds : MonoBehaviour
    {
        [Header("Lifetime")]
        [Tooltip("If true, a random lifetime between min and max is used.")]
        [SerializeField] private bool randomizeLifetime = false;

        [Tooltip("Lifetime in seconds when not randomizing.")]
        [SerializeField] private float lifetime = 2f;

        [Tooltip("Minimum lifetime when randomizing.")]
        [SerializeField] private float minLifetime = 1f;

        [Tooltip("Maximum lifetime when randomizing.")]
        [SerializeField] private float maxLifetime = 3f;

        [Tooltip("If true, destroys the GameObject if no pool is found.")]
        [SerializeField] private bool destroyIfNotPooled = true;

        private PooledObject _pooledObject;
        private IObjectPoolService _pool;
        private float _timer;
        private float _currentLifetime;

        private void Awake()
        {
            _pooledObject = GetComponent<PooledObject>();
            ServiceRegistry.TryResolve(out _pool);
        }

        private void OnEnable()
        {
            // Reset timer every time the pooled object is reused
            _timer = 0f;

            if (randomizeLifetime)
            {
                _currentLifetime = Random.Range(minLifetime, maxLifetime);
            }
            else
            {
                _currentLifetime = lifetime;
            }
        }

        private void Update()
        {
            _timer += UnityEngine.Time.deltaTime;

            if (_timer >= _currentLifetime)
            {
                Despawn();
            }
        }

        private void Despawn()
        {
            if (_pool != null)
            {
                _pool.Release(_pooledObject);
            }
            else if (destroyIfNotPooled)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
