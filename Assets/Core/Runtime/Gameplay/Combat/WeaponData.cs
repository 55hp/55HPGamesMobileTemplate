using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Combat
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "55HP/Combat/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float range = 5f;
        [SerializeField] private GameObject projectilePrefab = null;

        public float Damage => damage;
        public float FireRate => fireRate;
        public float Range => range;

        /// <summary>Null for melee/hitscan weapons that don't spawn a projectile.</summary>
        public GameObject ProjectilePrefab => projectilePrefab;
    }
}
