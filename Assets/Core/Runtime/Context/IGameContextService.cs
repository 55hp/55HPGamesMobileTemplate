namespace hp55games.Mobile.Core.Context
{
    /// <summary>
    /// Lightweight runtime game context.
    /// Stores current profile, run and level data for this session.
    /// Not responsible for persistence (that's SaveService's job).
    /// </summary>
    public interface IGameContextService
    {
        /// <summary>Current logical profile id (e.g. "default", "profile_1").</summary>
        string ProfileId { get; set; }

        /// <summary>Current level identifier (scene id, map id, etc.).</summary>
        string CurrentLevelId { get; set; }

        /// <summary>Current run seed (for roguelike / procedural runs).</summary>
        int CurrentRunSeed { get; set; }

        /// <summary>True if the game is running in debug/developer mode.</summary>
        bool IsDebug { get; set; }

        /// <summary>Current score for this run (optional, can be 0 if unused).</summary>
        int Score { get; set; }

        /// <summary>Current lives for this run (optional, can be 0 or -1 if unused).</summary>
        int Lives { get; set; }

        /// <summary>
        /// Resets all run-related transient data (score, lives, level, seed).
        /// Does NOT touch ProfileId or IsDebug.
        /// </summary>
        void ResetRun();
    }
}