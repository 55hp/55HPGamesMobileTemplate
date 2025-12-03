using UnityEngine;

namespace hp55games.Mobile.Core.Pooling
{
    public sealed class PooledObject : MonoBehaviour
    {
        [HideInInspector]
        public GameObject OriginPrefab;
    }
}