using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace hp55games.Mobile.Core.Analytics
{
    /// <summary>
    /// Default IAnalyticsService: no analytics SDK is integrated in the template, so every
    /// event is written to the console instead of being sent anywhere. Swap this
    /// registration in ServiceRegistry.InstallDefaults() for a real adapter (Firebase,
    /// GameAnalytics, Unity Analytics, ...) once one is integrated — no gameplay/UI call
    /// site needs to change.
    /// </summary>
    public sealed class DebugLogAnalyticsService : IAnalyticsService
    {
        public void LogEvent(string name, IDictionary<string, object> parameters = null)
        {
            Debug.Log($"[Analytics] {name}{FormatParams(parameters)}");
        }

        public void LevelStart(string levelId) =>
            LogEvent("level_start", new Dictionary<string, object> { ["level_id"] = levelId });

        public void LevelComplete(string levelId, int score) =>
            LogEvent("level_complete", new Dictionary<string, object> { ["level_id"] = levelId, ["score"] = score });

        public void LevelFail(string levelId, int score) =>
            LogEvent("level_fail", new Dictionary<string, object> { ["level_id"] = levelId, ["score"] = score });

        private static string FormatParams(IDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0) return string.Empty;
            return " { " + string.Join(", ", parameters.Select(kv => $"{kv.Key}={kv.Value}")) + " }";
        }
    }
}
