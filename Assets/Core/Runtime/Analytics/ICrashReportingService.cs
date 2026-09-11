using System;

namespace hp55games.Mobile.Core.Analytics
{
    /// <summary>
    /// SDK-agnostic crash/error reporting. Placed alongside IAnalyticsService rather than a
    /// new Diagnostics/ folder: crash reporting is the same kind of concern as analytics
    /// events here — "send data about what happened to a third-party dashboard" — just a
    /// different SDK surface and payload shape, not a different architectural layer.
    /// Mirrors Firebase.Crashlytics.Crashlytics's surface but stays SDK-agnostic like every
    /// other service interface in this template — gameplay/UI code never references Firebase
    /// directly, only this interface.
    /// </summary>
    public interface ICrashReportingService
    {
        void LogException(Exception ex);

        void SetCustomKey(string key, string value);

        void Log(string message);
    }
}
