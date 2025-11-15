using UnityEngine;

namespace hp55games.Mobile.Core.Pooling
{
    /// <summary>
    /// Global object pool service.
    /// Reuses GameObjects instead of destroying/instantiating each time.
    /// </summary>
    public interface IObjectPoolService
    {
        /// <summary>
        /// Returns an instance of the given prefab at the requested position/rotation.
        /// If an inactive instance is available in the pool, it is reused.
        /// Otherwise, a new one is instantiated.
        /// </summary>
        GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);

        /// <summary>
        /// Returns the instance to the pool (deactivates it). 
        /// If the instance was not created by the pool, it is simply Destroy()'d.
        /// </summary>
        void Despawn(GameObject instance);
    }
}