using UnityEngine;
using hp55games.Mobile.Core.Gameplay.Damage;

namespace hp55games.Mobile.Core.Gameplay.Combat
{
    [CreateAssetMenu(fileName = "NewAttackData", menuName = "55HP/Combat/Attack Data")]
    public class AttackData : ScriptableObject
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float knockback = 0f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        [SerializeField] private float hitboxActiveDuration = 0.2f;

        public float Damage => damage;
        public float Knockback => knockback;
        public DamageType DamageType => damageType;
        public float HitboxActiveDuration => hitboxActiveDuration;
    }
}
