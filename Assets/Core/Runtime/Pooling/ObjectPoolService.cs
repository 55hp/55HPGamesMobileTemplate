using System.Collections.Generic;
using UnityEngine;

namespace hp55games.Mobile.Core.Pooling
{
    /// <summary>
    /// Simple object pool implementation keyed by prefab.
    /// - Reuses inactive instances instead of destroying them.
    /// - If an instance was not created by this pool, Despawn() will Destroy() it.
    /// </summary>
    public sealed class ObjectPoolService : IObjectPoolService
    {
        // Prefab -> queue of inactive instances
        private readonly Dictionary<GameObject, Queue<GameObject>> _pool = new();

        // Instance -> prefab (to know which queue to return it to)
        private readonly Dictionary<GameObject, GameObject> _instanceToPrefab = new();

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("[ObjectPoolService] Spawn called with null prefab.");
                return null;
            }

            GameObject instance = null;

            // Try to reuse from pool
            if (_pool.TryGetValue(prefab, out var queue) && queue.Count > 0)
            {
                instance = queue.Dequeue();
                if (instance == null)
                {
                    // just in case something was destroyed externally
                    return Spawn(prefab, position, rotation, parent);
                }
            }
            else
            {
                // No pool for this prefab yet or empty -> create new
                instance = Object.Instantiate(prefab);
                _instanceToPrefab[instance] = prefab;

                // Ensure it has PooledObject
                var po = instance.GetComponent<PooledObject>();
                if (po == null)
                    po = instance.AddComponent<PooledObject>();

                po.Prefab = prefab;
            }

            // Setup transform and activate
            var tr = instance.transform;
            tr.SetParent(parent, worldPositionStays: false);
            tr.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);

            return instance;
        }

        public void Despawn(GameObject instance)
        {
            if (instance == null)
                return;

            // Is this instance managed by the pool?
            if (!_instanceToPrefab.TryGetValue(instance, out var prefab) || prefab == null)
            {
                // Not managed -> destroy it
                Object.Destroy(instance);
                return;
            }

            // Deactivate and put back in the queue
            instance.SetActive(false);
            instance.transform.SetParent(null, worldPositionStays: false);

            if (!_pool.TryGetValue(prefab, out var queue))
            {
                queue = new Queue<GameObject>();
                _pool[prefab] = queue;
            }

            queue.Enqueue(instance);
        }
    }
}
