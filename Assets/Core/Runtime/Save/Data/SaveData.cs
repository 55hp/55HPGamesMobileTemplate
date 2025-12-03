using System;
using System.Collections.Generic;

namespace hp55games.Mobile.Core.Save
{
    [Serializable]
    public class SaveData
    {
        public int coins;
        public string lastProfile = "default";
        public OptionsData options = new OptionsData();

        /// <summary>
        /// Generic player progress (best scores, records, stats).
        /// </summary>
        public PlayerProgressData progress = new PlayerProgressData();

        public List<TimeStampEntry> timeStamps = new List<TimeStampEntry>();

        public string lastUtcIso;
        public double lastMonotonicSeconds;
    }
}