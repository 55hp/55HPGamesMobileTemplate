#if FIREBASE_CRASHLYTICS_ENABLED
using System;
using Firebase.Crashlytics;

namespace hp55games.Mobile.Core.Analytics
{
    /// <summary>
    /// ICrashReportingService backed by Firebase.Crashlytics.Crashlytics.
    ///
    /// Compiled ONLY when FIREBASE_CRASHLYTICS_ENABLED is defined (Project Settings > Player >
    /// Scripting Define Symbols) — same reasoning as FirebaseAnalyticsService: this keeps the
    /// file out of the default Core.Runtime compile set until a concrete project has actually
    /// imported the Firebase Crashlytics SDK, so the template doesn't fail to compile for
    /// every project that hasn't set up Firebase yet. Not registered by
    /// ServiceRegistry.InstallDefaults() for the same per-project-config reason as
    /// FirebaseAnalyticsService.
    /// </summary>
    public sealed class FirebaseCrashReportingService : ICrashReportingService
    {
        public void LogException(Exception ex) => Crashlytics.LogException(ex);

        public void SetCustomKey(string key, string value) => Crashlytics.SetCustomKey(key, value);

        public void Log(string message) => Crashlytics.Log(message);
    }
}
#endif
