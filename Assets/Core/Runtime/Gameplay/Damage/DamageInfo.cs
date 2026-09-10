using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Damage
{
    /// <summary>
    /// Payload delivered to IHealthSystem.ApplyDamage. Adds DamageType and a named
    /// hit point on top of what DamageContext carries, for HP-based entities that
    /// need more than the fatal/non-fatal contact model ContactDamager provides.
    /// </summary>
    public readonly struct DamageInfo
    {
        public readonly float Amount;
        public readonly DamageType Type;
        public readonly GameObject Source;
        public readonly Vector3 HitPoint;

        public DamageInfo(float amount, DamageType type, GameObject source, Vector3 hitPoint)
        {
            Amount = amount;
            Type = type;
            Source = source;
            HitPoint = hitPoint;
        }

        /// <summary>Maps a DamageContext (from ContactDamager) into a DamageInfo. IsFatal is not
        /// representable here — callers bridging from IDamageable.TakeDamage must resolve it themselves.</summary>
        public static DamageInfo FromContext(DamageContext ctx, DamageType type = DamageType.Physical)
        {
            return new DamageInfo(ctx.Amount, type, ctx.Source, ctx.ContactPoint);
        }
    }
}
