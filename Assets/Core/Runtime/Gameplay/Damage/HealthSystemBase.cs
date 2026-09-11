using System;
using System.Collections;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.Damage
{
    /// <summary>
    /// Base HP tracker for concrete health components. Also implements IDamageable so
    /// existing ContactDamager sources can drive it: a fatal contact is translated into
    /// damage equal to CurrentHealth, guaranteeing death while still respecting invulnerability.
    /// </summary>
    public abstract class HealthSystemBase : MonoBehaviour, IHealthSystem, IDamageable
    {
        [SerializeField]
        private float maxHealth = 100f;

        [SerializeField]
        [Tooltip("Seconds of invulnerability granted after taking damage. 0 disables i-frames.")]
        private float invulnerabilityDuration = 0f;

        private float _currentHealth;
        private bool _isDead;
        private Coroutine _invulnerabilityRoutine;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => !_isDead;
        public bool IsInvulnerable { get; private set; }

        public event Action<DamageInfo> OnDamaged;
        public event Action OnDeath;

        protected virtual void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void ApplyDamage(DamageInfo info)
        {
            if (!IsAlive || IsInvulnerable)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - info.Amount, 0f, maxHealth);

            OnDamageApplied(info);
            OnDamaged?.Invoke(info);

            if (invulnerabilityDuration > 0f)
                StartInvulnerability(invulnerabilityDuration);

            if (_currentHealth <= 0f && !_isDead)
            {
                _isDead = true;
                OnDied();
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth + amount, 0f, maxHealth);
        }

        void IDamageable.TakeDamage(DamageContext ctx)
        {
            // Resolve the fatal-contact override (raw HP-drop amount) before handing off to
            // FromContext, which otherwise just forwards ctx.Amount as-is.
            float amount = ctx.IsFatal ? _currentHealth : ctx.Amount;
            var resolvedCtx = new DamageContext(ctx.Source, ctx.ContactPoint, amount, false);
            ApplyDamage(DamageInfo.FromContext(resolvedCtx, DamageType.Physical));
        }

        /// <summary>Hook for subclasses reacting to damage (VFX, sound, camera shake, ...).</summary>
        protected virtual void OnDamageApplied(DamageInfo info) { }

        /// <summary>Hook for subclasses reacting to death (ragdoll, drop loot, disable, ...).</summary>
        protected virtual void OnDied() { }

        private void StartInvulnerability(float duration)
        {
            if (_invulnerabilityRoutine != null)
                StopCoroutine(_invulnerabilityRoutine);
            _invulnerabilityRoutine = StartCoroutine(InvulnerabilityRoutine(duration));
        }

        private IEnumerator InvulnerabilityRoutine(float duration)
        {
            IsInvulnerable = true;
            yield return new WaitForSeconds(duration);
            IsInvulnerable = false;
            _invulnerabilityRoutine = null;
        }
    }
}
