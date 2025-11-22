using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SaveService;

namespace hp55games.Mobile.Core.Time
{
    /// <summary>
    /// TimeService livello "Mobile Standard":
    /// - Espone ora UTC / locale / monotonic
    /// - Tiene traccia di timestamp logici (per daily, cooldown, ecc.)
    /// - Rileva clock-back (orologio di sistema spostato indietro > 2 minuti)
    ///   e clampa eventuali delta negativi.
    /// </summary>
    public sealed class TimeService : ITimeService
    {
        private readonly ISaveService _save;
        private readonly SaveData _data;
        private bool _clockSuspicious;

        public TimeService()
        {
            _save = ServiceRegistry.Resolve<ISaveService>();
            _data = _save.Data;

            CheckClock();
        }

        // ----------------- Proprietà base -----------------

        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime LocalNow => DateTime.Now;

        public double UnscaledGameTime => UnityEngine.Time.unscaledTime;
        public double MonotonicSeconds => UnityEngine.Time.realtimeSinceStartupAsDouble;

        public bool IsClockSuspicious => _clockSuspicious;

        // ----------------- API timestamp logici -----------------

        public DateTime GetLastUtc(string key)
        {
            if (string.IsNullOrEmpty(key))
                return DateTime.MinValue;

            var entry = FindEntry(key, createIfMissing: false);
            if (entry == null || string.IsNullOrEmpty(entry.isoUtc))
                return DateTime.MinValue;

            if (DateTime.TryParse(entry.isoUtc, null, DateTimeStyles.RoundtripKind, out var parsed))
                return parsed;

            return DateTime.MinValue;
        }

        public void SetNowUtc(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            var entry = FindEntry(key, createIfMissing: true);
            entry.isoUtc = UtcNow.ToString("o");
            _save.Save();
        }

        public TimeSpan SinceLast(string key)
        {
            var last = GetLastUtc(key);
            if (last == DateTime.MinValue)
                return TimeSpan.Zero;

            var now = UtcNow;
            var delta = now - last;

            if (_clockSuspicious || delta < TimeSpan.Zero)
                return TimeSpan.Zero;

            return delta;
        }

        public bool HasPassed(string key, TimeSpan threshold)
        {
            return SinceLast(key) >= threshold;
        }

        // ----------------- Helpers interni -----------------

        private TimeStampEntry FindEntry(string key, bool createIfMissing)
        {
            if (_data.timeStamps == null)
                _data.timeStamps = new System.Collections.Generic.List<TimeStampEntry>();

            var entry = _data.timeStamps.FirstOrDefault(e => e.key == key);
            if (entry == null && createIfMissing)
            {
                entry = new TimeStampEntry { key = key, isoUtc = null };
                _data.timeStamps.Add(entry);
            }

            return entry;
        }

        private void CheckClock()
        {
            var nowUtc  = DateTime.UtcNow;
            var nowMono = UnityEngine.Time.realtimeSinceStartupAsDouble;

            if (!string.IsNullOrEmpty(_data.lastUtcIso))
            {
                if (DateTime.TryParse(_data.lastUtcIso, null, DateTimeStyles.RoundtripKind, out var prevUtc))
                {
                    var diffMinutes = (nowUtc - prevUtc).TotalMinutes;
                    if (diffMinutes < -2.0)
                    {
                        _clockSuspicious = true;
                        Debug.LogWarning($"[TimeService] Clock-back rilevato. diffMinutes={diffMinutes:F2}");
                    }
                }
            }

            _data.lastUtcIso           = nowUtc.ToString("o");
            _data.lastMonotonicSeconds = nowMono;
            _save.Save();
        }
    }
}
