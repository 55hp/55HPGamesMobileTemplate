using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.StatusEffects
{
    [CreateAssetMenu(fileName = "NewStatusEffect", menuName = "55HP/Combat/Status Effect")]
    public class StatusEffectData : ScriptableObject
    {
        [SerializeField] private StatusEffectType type = StatusEffectType.Burn;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float tickInterval = 1f;
        [SerializeField] private float modifier = 0f;
        [SerializeField] private StackPolicy stackPolicy = StackPolicy.Refresh;
        [SerializeField] private GameObject vfxPrefab = null;

        public StatusEffectType Type => type;
        public float Duration => duration;
        public float TickInterval => tickInterval;
        public float Modifier => modifier;
        public StackPolicy StackPolicy => stackPolicy;

        /// <summary>Null when the effect has no dedicated VFX.</summary>
        public GameObject VfxPrefab => vfxPrefab;
    }
}
