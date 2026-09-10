using System;

namespace hp55games.Mobile.Core.Gameplay.Damage
{
    /// <summary>
    /// HP-based entity contract. Distinct from IDamageable: IDamageable is the generic
    /// "can receive a contact" seam used by ContactDamager, IHealthSystem is the richer
    /// HP/invulnerability/death model for entities that track health explicitly.
    /// </summary>
    public interface IHealthSystem
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        bool IsInvulnerable { get; }

        void ApplyDamage(DamageInfo info);
        void Heal(float amount);

        event Action<DamageInfo> OnDamaged;
        event Action OnDeath;
    }
}
