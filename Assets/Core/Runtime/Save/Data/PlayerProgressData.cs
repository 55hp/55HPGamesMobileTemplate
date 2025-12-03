using System;

namespace hp55games.Mobile.Core.Save
{
    /// <summary>
    /// Generic container for player progress and records.
    /// This is shared across all games that use the 55HP core template.
    /// </summary>
    [Serializable]
    public class PlayerProgressData
    {
        /// <summary>
        /// Global best score across runs (used by FlappyTest, but generic).
        /// </summary>
        public int bestScore = 0;

        /// <summary>
        /// Highest level / stage reached (future use).
        /// </summary>
        public int highestLevel = 0;

        /// <summary>
        /// Lifetime coins earned across all sessions (future use).
        /// </summary>
        public int lifetimeCoins = 0;

        // In futuro puoi aggiungere:
        // public int totalRunsPlayed;
        // public int enemiesKilled;
        // public int questsCompleted;
    }
}