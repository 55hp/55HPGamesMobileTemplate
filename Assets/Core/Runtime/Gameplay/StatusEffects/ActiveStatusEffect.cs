namespace hp55games.Mobile.Core.Gameplay.StatusEffects
{
    /// <summary>Runtime instance of a StatusEffectData applied to a single target. Plain class
    /// (not a MonoBehaviour) since it's owned and ticked by an IStatusEffectHandler, not the scene graph.</summary>
    public class ActiveStatusEffect
    {
        public StatusEffectData Data;
        public float RemainingDuration;
        public float TimeSinceLastTick;

        public ActiveStatusEffect(StatusEffectData data)
        {
            Data = data;
            RemainingDuration = data.Duration;
            TimeSinceLastTick = 0f;
        }
    }
}
