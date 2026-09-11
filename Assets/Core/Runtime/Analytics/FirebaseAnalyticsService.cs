#if FIREBASE_ANALYTICS_ENABLED
using System.Collections.Generic;
using Firebase.Analytics;

namespace hp55games.Mobile.Core.Analytics
{
    /// <summary>
    /// IAnalyticsService backed by Firebase.Analytics.FirebaseAnalytics.
    ///
    /// Compiled ONLY when FIREBASE_ANALYTICS_ENABLED is defined (Project Settings > Player >
    /// Scripting Define Symbols). This template ships without the Firebase Unity SDK — it's a
    /// per-project download, not a package this repo can vendor — so this file must not be
    /// part of the default Core.Runtime compile set until a concrete project has actually
    /// imported Firebase.Analytics. Without this guard, importing the template without
    /// Firebase would fail to compile the entire Core.Runtime assembly (every other script in
    /// the template lives in it), not just this file. Define the symbol yourself once
    /// Firebase.Analytics is present in the project.
    ///
    /// Assumes Firebase.FirebaseApp.CheckAndFixDependenciesAsync() has already completed
    /// successfully elsewhere in the host project's startup — this class only wraps SDK call
    /// shapes, it does not own Firebase's own init lifecycle. Not registered by
    /// ServiceRegistry.InstallDefaults() — Firebase requires a per-project
    /// google-services.json/GoogleService-Info.plist the template can't ship, so whoever sets
    /// up Firebase for their concrete project registers this themselves.
    /// </summary>
    public sealed class FirebaseAnalyticsService : IAnalyticsService
    {
        public void LogEvent(string name, IDictionary<string, object> parameters = null)
        {
            if (parameters == null || parameters.Count == 0)
            {
                FirebaseAnalytics.LogEvent(name);
                return;
            }

            var fbParams = new Parameter[parameters.Count];
            int i = 0;
            foreach (var kv in parameters)
                fbParams[i++] = ToParameter(kv.Key, kv.Value);

            FirebaseAnalytics.LogEvent(name, fbParams);
        }

        public void LevelStart(string levelId) =>
            LogEvent("level_start", new Dictionary<string, object> { ["level_id"] = levelId });

        public void LevelComplete(string levelId, int score) =>
            LogEvent("level_complete", new Dictionary<string, object> { ["level_id"] = levelId, ["score"] = score });

        public void LevelFail(string levelId, int score) =>
            LogEvent("level_fail", new Dictionary<string, object> { ["level_id"] = levelId, ["score"] = score });

        private static Parameter ToParameter(string key, object value)
        {
            switch (value)
            {
                case int i: return new Parameter(key, i);
                case long l: return new Parameter(key, l);
                case float f: return new Parameter(key, (double)f);
                case double d: return new Parameter(key, d);
                default: return new Parameter(key, value?.ToString() ?? string.Empty);
            }
        }
    }
}
#endif
