using System;

namespace hp55games.Mobile.Core.Save
{
    [Serializable]
    public class TimeStampEntry
    {
        public string key;
        public string isoUtc; // DateTime in formato "o" (ISO 8601)
    }
}