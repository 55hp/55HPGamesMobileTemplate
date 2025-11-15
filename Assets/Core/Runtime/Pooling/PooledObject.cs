using UnityEngine;

namespace hp55games.Mobile.Core.Pooling
{
    /// <summary>
    /// Component used internally by the pool to remember which prefab an instance belongs to.
    /// You don't need to add this manually; the pool will add it when instantiating.
    /// </summary>
    public sealed class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// The original prefab this instance was created from.
        /// </summary>
        public GameObject Prefab { get; set; }
    }
}