using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace hp55games.Mobile.Core.SaveService
{
    [Serializable]
    public class OptionsData
    {
        public float music = 0.8f;
        public float sfx   = 0.8f;
        public bool  hapt  = true;
        public string lang = "en";
        public bool  musicMute = false;
        public bool  sfxMute   = false;
    }

    [Serializable]
    public class TimeStampEntry
    {
        public string key;
        public string isoUtc; // DateTime in formato "o" (ISO 8601)
    }

    [Serializable]
    public class SaveData
    {
        public int coins;
        public string lastProfile = "default";

        public OptionsData options = new OptionsData();

        // --- TimeService data ---
        public List<TimeStampEntry> timeStamps = new List<TimeStampEntry>();

        // Per controllo base "clock back" / monotonic
        public string lastUtcIso;
        public double lastMonotonicSeconds;
    }
}

namespace hp55games.Mobile.Core.Architecture
{
    public interface ISaveService
    {
        hp55games.Mobile.Core.SaveService.SaveData Data { get; }
        void Load();
        void Save();
    }

    public sealed class SaveService : ISaveService
    {
        private const string FileName = "save.json";
        public hp55games.Mobile.Core.SaveService.SaveData Data { get; private set; } = new();

        public void Load()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                Data = JsonUtility.FromJson<hp55games.Mobile.Core.SaveService.SaveData>(
                    File.ReadAllText(path)
                );
            }
            else
            {
                Save(); // crea file con default
            }
        }

        public void Save()
        {
            var path  = Path.Combine(Application.persistentDataPath, FileName);
            var json  = JsonUtility.ToJson(Data);
            File.WriteAllText(path, json);
        }
    }
}
