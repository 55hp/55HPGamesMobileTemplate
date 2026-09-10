using System;
using System.Collections.Generic;

namespace hp55games.Mobile.Core.Gameplay.StatusEffects
{
    public interface IStatusEffectHandler
    {
        void ApplyEffect(StatusEffectData effect);
        void RemoveEffect(StatusEffectType type);
        bool HasEffect(StatusEffectType type);
        IReadOnlyList<ActiveStatusEffect> ActiveEffects { get; }

        event Action<StatusEffectType> OnEffectApplied;
        event Action<StatusEffectType> OnEffectExpired;
    }
}
