using System.Collections.Generic;

namespace hp55games.Mobile.Core.Analytics
{
    /// <summary>
    /// SDK-agnostic event tracking. Gameplay/UI code depends only on this interface, never
    /// on a specific analytics SDK (Firebase, GameAnalytics, Unity Analytics, ...) — see
    /// DebugLogAnalyticsService for the template's default implementation.
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>Logs a named event with optional parameters.</summary>
        void LogEvent(string name, IDictionary<string, object> parameters = null);

        /// <summary>Convention helper: a run/level started.</summary>
        void LevelStart(string levelId);

        /// <summary>Convention helper: a run/level was completed successfully.</summary>
        void LevelComplete(string levelId, int score);

        /// <summary>Convention helper: a run/level ended in failure.</summary>
        void LevelFail(string levelId, int score);
    }
}
