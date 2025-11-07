// Assets/Core/Runtime/Save/SaveService.cs

using System;
using System.IO;
using UnityEngine;

namespace hp55games.Mobile.Core.SaveService
{
    [Serializable]
    public class SaveData
    {
        public int coins;
        public string lastProfile = "default";
        
        public OptionsData options = new OptionsData(); 
    }
    
    [Serializable]
    public class OptionsData
    {
        public float music = 0.8f;
        public float sfx   = 0.8f;
        public bool  hapt  = true;
        public string lang = "en";
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
                Data = JsonUtility.FromJson<hp55games.Mobile.Core.SaveService.SaveData>(File.ReadAllText(path));
            else
                Save();
        }

        public void Save()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            File.WriteAllText(path, JsonUtility.ToJson(Data));
        }
    }
}

