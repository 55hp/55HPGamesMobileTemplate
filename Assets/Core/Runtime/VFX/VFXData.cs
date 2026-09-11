using UnityEngine;
using hp55games.Mobile.Core.Pooling;

namespace hp55games.Mobile.Core.VFX
{
    /// <summary>
    /// Static definition of a visual effect. prefab is typed as PooledObject, not the more
    /// obvious GameObject, because IObjectPoolService.Get/Release operate on PooledObject —
    /// typing it that way here lets an IVFXService implementation hand this field straight to
    /// IObjectPoolService.Get(prefab, parent) with no lookup/cast step in between.
    /// </summary>
    [CreateAssetMenu(fileName = "NewVFXData", menuName = "55HP/VFX/VFX Data")]
    public class VFXData : ScriptableObject
    {
        [SerializeField] private PooledObject prefab = null;

        [Tooltip("Informational for one-shot effects with no natural end signal from the " +
                 "particle system itself. IVFXService implementations may or may not use this " +
                 "to drive auto-release, depending on how they detect completion.")]
        [SerializeField] private float duration = 1f;

        public PooledObject Prefab => prefab;
        public float Duration => duration;
    }
}
