using System;

namespace hp55games.Mobile.Core.Time
{
    public interface ITimeService
    {
        /// <summary>Ora corrente in UTC, direttamente da DateTime.UtcNow.</summary>
        DateTime UtcNow { get; }

        /// <summary>Ora corrente locale (DateTime.Now).</summary>
        DateTime LocalNow { get; }

        /// <summary>Tempo di gioco non scalato (Time.unscaledTime).</summary>
        double UnscaledGameTime { get; }

        /// <summary>Clock monotono basato su Time.realtimeSinceStartupAsDouble.</summary>
        double MonotonicSeconds { get; }

        /// <summary>Ultimo timestamp registrato per una certa chiave, oppure DateTime.MinValue se assente.</summary>
        DateTime GetLastUtc(string key);

        /// <summary>Imposta il timestamp per una chiave all'ora UTC corrente.</summary>
        void SetNowUtc(string key);

        /// <summary>Quanto tempo è passato dall'ultima volta (Utc) per quella chiave. Se nulla, TimeSpan.Zero.</summary>
        TimeSpan SinceLast(string key);

        /// <summary>Ritorna true se per la chiave è passato almeno threshold.</summary>
        bool HasPassed(string key, TimeSpan threshold);

        /// <summary>True se è stato rilevato un "clock back" significativo.</summary>
        bool IsClockSuspicious { get; }
    }
}