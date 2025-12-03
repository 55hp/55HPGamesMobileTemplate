using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Pooling;

namespace hp55games.Mobile.Core.Gameplay
{
    /// <summary>
    /// Generic 2D spawner that periodically gets instances from IObjectPoolService
    /// and places them at this transform position, with optional random Y offset.
    /// Supports initial delay and random spawn intervals.
    /// </summary>
    public sealed class TimedPooledSpawner2D : MonoBehaviour
    {
        [Header("Pool / Prefab")]
        [SerializeField] private PooledObject _prefab;
        [SerializeField] private int _initialPoolSize = 8;
        [SerializeField] private Transform _parentForInstances;

        [Header("Start / Stop")]
        [SerializeField] private bool _startActive = true;
        [SerializeField] private float _initialDelay = 0f;

        [Header("Timing")]
        [SerializeField] private float _spawnInterval = 1.5f;

        [SerializeField] private bool _useRandomInterval = false;
        [SerializeField] private float _minInterval = 1.0f;
        [SerializeField] private float _maxInterval = 2.0f;

        [Header("Position / Randomization")]
        [SerializeField] private bool _randomizeY = false;
        [SerializeField] private float _minY = -1f;
        [SerializeField] private float _maxY = 1f;
        
        [SerializeField] private bool _randomizeX = false;
        [SerializeField] private float _minX = -1f;
        [SerializeField] private float _maxX = 1f;

        private IObjectPoolService _pool;

        private bool _isSpawning;
        private float _time;
        private float _nextSpawnAt;

        private void Awake()
        {
            ServiceRegistry.TryResolve(out _pool);

            if (_parentForInstances == null)
                _parentForInstances = transform;

            if (_pool != null && _prefab != null && _initialPoolSize > 0)
            {
                _pool.WarmUp(_prefab, _initialPoolSize, _parentForInstances);
            }

            _time = 0f;
            _isSpawning = _startActive;

            // Decidiamo quando deve avvenire il primo spawn
            if (_startActive)
            {
                _nextSpawnAt = (_initialDelay <= 0f)
                    ? 0f                       // spawn immediato
                    : _initialDelay;           // primo spawn dopo initialDelay
            }
        }

        private void Update()
        {
            if (_pool == null || _prefab == null)
                return;

            if (!_isSpawning)
                return;

            _time += UnityEngine.Time.deltaTime;

            if (_time < _nextSpawnAt)
                return;

            SpawnOne();

            // Programma il prossimo spawn
            _nextSpawnAt = _time + GetNextInterval();
        }

        private float GetNextInterval()
        {
            if (!_useRandomInterval)
                return Mathf.Max(0.01f, _spawnInterval);

            var min = Mathf.Max(0.01f, _minInterval);
            var max = Mathf.Max(min, _maxInterval);

            return Random.Range(min, max);
        }

        private void SpawnOne()
        {
            var go = _pool.Get(_prefab, _parentForInstances);
            var tr = go.transform;

            var basePos = transform.position;
            float y = basePos.y;
            float x = basePos.x;

            if (_randomizeY)
            {
                y = Random.Range(_minY, _maxY);
            }

            if (_randomizeX)
            {
                x = Random.Range(_minX, _maxX);
            }

            tr.position = new Vector3(x, y, basePos.z);
            go.SetActive(true);
        }

        // Facoltativi ma utili per controllare da codice

        public void StartSpawning()
        {
            if (_isSpawning)
                return;

            _isSpawning = true;

            // Se non avevamo ancora schedulato il primo spawn, fallo ora
            if (_nextSpawnAt <= 0f && _time <= 0f)
            {
                _nextSpawnAt = (_initialDelay <= 0f) ? 0f : _initialDelay;
            }
        }

        public void StopSpawning()
        {
            _isSpawning = false;
        }
    }
}