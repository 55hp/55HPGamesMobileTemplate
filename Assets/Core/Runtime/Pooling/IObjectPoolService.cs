using UnityEngine;

namespace hp55games.Mobile.Core.Pooling
{
    /// <summary>
    /// Generic object pooling service for any prefab-based object.
    /// Works with MonoBehaviours, but you call it by prefab reference.
    /// </summary>
    public interface IObjectPoolService
    {
        /// <summary>
        /// Ensure there are at least 'count' instances pre-created (warm-up).
        /// </summary>
        void WarmUp(PooledObject prefab, int count, Transform parent = null);

        /// <summary>
        /// Get (or spawn) an instance of the given prefab.
        /// </summary>
        GameObject Get(PooledObject prefab, Transform parent = null);

        /// <summary>
        /// Release an instance back to its pool.
        /// If 'instance' was not created by the pool, you MAY Destroy it (optional).
        /// </summary>
        void Release(PooledObject instance);

        /// <summary>
        /// Clear all instances (for scene change / tests).
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Clear only one prefab pool (optional ma molto comodo).
        /// </summary>
        void ClearPrefabPool(PooledObject prefab);
    }
}