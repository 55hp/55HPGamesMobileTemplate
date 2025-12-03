using System.Collections.Generic;
using UnityEngine;

namespace hp55games.Mobile.Core.Pooling
{
    public sealed class ObjectPoolService : IObjectPoolService
    {
        // Chiave = prefab GameObject (non l'istanza)
        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();

        public void WarmUp(PooledObject prefab, int count, Transform parent = null)
        {
            if (count <= 0 || prefab == null) return;

            for (int i = 0; i < count; i++)
            {
                var go = Get(prefab, parent);
                var po = go.GetComponent<PooledObject>();
                Release(po);
            }
        }

        public GameObject Get(PooledObject prefab, Transform parent = null)
        {
            if (prefab == null)
            {
                Debug.LogError("[ObjectPoolService] Get called with null prefab.");
                return null;
            }

            var prefabGo = prefab.gameObject;

            if (!_pools.TryGetValue(prefabGo, out var pool))
            {
                pool = new Queue<GameObject>();
                _pools[prefabGo] = pool;
            }

            GameObject instance = null;

            while (pool.Count > 0 && instance == null)
            {
                instance = pool.Dequeue();
            }

            if (instance == null)
            {
                instance = Object.Instantiate(prefabGo, parent);
                var po = instance.GetComponent<PooledObject>();
                if (po == null) po = instance.AddComponent<PooledObject>();
                po.OriginPrefab = prefabGo;
            }
            else
            {
                instance.transform.SetParent(parent, false);
                instance.SetActive(true);
            }

            return instance;
        }

        public void Release(PooledObject instance)
        {
            if (instance == null) return;

            var go = instance.gameObject;
            var origin = instance.OriginPrefab;

            if (origin == null)
            {
                // Non sappiamo a quale pool appartiene → distruggiamo l'istanza
                Object.Destroy(go);
                return;
            }

            if (!_pools.TryGetValue(origin, out var pool))
            {
                pool = new Queue<GameObject>();
                _pools[origin] = pool;
            }

            go.SetActive(false);
            go.transform.SetParent(null);
            pool.Enqueue(go);
        }

        public void ClearAll()
        {
            foreach (var kvp in _pools)
            {
                foreach (var go in kvp.Value)
                {
                    if (go != null) Object.Destroy(go);
                }
            }
            _pools.Clear();
        }

        public void ClearPrefabPool(PooledObject prefab)
        {
            if (prefab == null) return;

            var prefabGo = prefab.gameObject;
            if (!_pools.TryGetValue(prefabGo, out var pool)) return;

            foreach (var go in pool)
            {
                if (go != null) Object.Destroy(go);
            }

            _pools.Remove(prefabGo);
        }
    }
}
