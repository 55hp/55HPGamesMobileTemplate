namespace hp55games.Mobile.Core.Gameplay.StatusEffects
{
    public enum StackPolicy
    {
        /// <summary>Reapplication adds to the existing effect's remaining duration.</summary>
        Stack,

        /// <summary>Reapplication resets the existing effect's remaining duration.</summary>
        Refresh,

        /// <summary>Reapplication replaces the existing effect's data and remaining duration.</summary>
        Replace
    }
}
