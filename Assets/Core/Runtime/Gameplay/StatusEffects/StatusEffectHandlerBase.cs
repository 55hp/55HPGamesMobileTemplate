using System;
using System.Collections.Generic;
using UnityEngine;

namespace hp55games.Mobile.Core.Gameplay.StatusEffects
{
    /// <summary>
    /// Base status-effect tracker. Holds at most one ActiveStatusEffect per StatusEffectType;
    /// StackPolicy governs what happens when the same type is (re)applied while already active.
    /// </summary>
    public abstract class StatusEffectHandlerBase : MonoBehaviour, IStatusEffectHandler
    {
        private readonly Dictionary<StatusEffectType, ActiveStatusEffect> _activeByType =
            new Dictionary<StatusEffectType, ActiveStatusEffect>();

        private readonly List<ActiveStatusEffect> _activeEffects = new List<ActiveStatusEffect>();

        // Reused each Update to avoid allocating a new list per frame while removing expired entries.
        private readonly List<StatusEffectType> _expiredBuffer = new List<StatusEffectType>();

        public IReadOnlyList<ActiveStatusEffect> ActiveEffects => _activeEffects;

        public event Action<StatusEffectType> OnEffectApplied;
        public event Action<StatusEffectType> OnEffectExpired;

        public void ApplyEffect(StatusEffectData effect)
        {
            if (effect == null)
                return;

            if (_activeByType.TryGetValue(effect.Type, out ActiveStatusEffect existing))
            {
                switch (effect.StackPolicy)
                {
                    case StackPolicy.Stack:
                        existing.RemainingDuration += effect.Duration;
                        break;

                    case StackPolicy.Refresh:
                        existing.RemainingDuration = effect.Duration;
                        break;

                    case StackPolicy.Replace:
                        existing.Data = effect;
                        existing.RemainingDuration = effect.Duration;
                        existing.TimeSinceLastTick = 0f;
                        break;
                }

                OnEffectApplied?.Invoke(effect.Type);
                return;
            }

            var active = new ActiveStatusEffect(effect);
            _activeByType.Add(effect.Type, active);
            _activeEffects.Add(active);

            OnEffectStarted(effect);
            OnEffectApplied?.Invoke(effect.Type);
        }

        public void RemoveEffect(StatusEffectType type)
        {
            if (!_activeByType.TryGetValue(type, out ActiveStatusEffect active))
                return;

            _activeByType.Remove(type);
            _activeEffects.Remove(active);

            OnEffectEnded(type);
            OnEffectExpired?.Invoke(type);
        }

        public bool HasEffect(StatusEffectType type) => _activeByType.ContainsKey(type);

        protected virtual void Update()
        {
            if (_activeEffects.Count == 0)
                return;

            float deltaTime = Time.deltaTime;
            _expiredBuffer.Clear();

            for (int i = 0; i < _activeEffects.Count; i++)
            {
                ActiveStatusEffect active = _activeEffects[i];
                active.RemainingDuration -= deltaTime;
                active.TimeSinceLastTick += deltaTime;

                float tickInterval = active.Data.TickInterval;
                if (tickInterval > 0f && active.TimeSinceLastTick >= tickInterval)
                {
                    active.TimeSinceLastTick -= tickInterval;
                    OnEffectTick(active.Data);
                }

                if (active.RemainingDuration <= 0f)
                    _expiredBuffer.Add(active.Data.Type);
            }

            for (int i = 0; i < _expiredBuffer.Count; i++)
                RemoveEffect(_expiredBuffer[i]);
        }

        /// <summary>Hook fired once when a new effect instance becomes active (VFX spawn, initial stat apply, ...).</summary>
        protected virtual void OnEffectStarted(StatusEffectData effect) { }

        /// <summary>Hook fired every TickInterval seconds while the effect is active (DoT damage, periodic heal, ...).</summary>
        protected virtual void OnEffectTick(StatusEffectData effect) { }

        /// <summary>Hook fired when the effect's duration expires or it is removed early (VFX teardown, stat revert, ...).</summary>
        protected virtual void OnEffectEnded(StatusEffectType type) { }
    }
}
