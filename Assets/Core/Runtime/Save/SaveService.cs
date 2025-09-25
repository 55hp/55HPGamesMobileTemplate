// Assets/Core/Runtime/Save/SaveService.cs
using System.IO;
using UnityEngine;

namespace hp55games.Mobile.Core.Save
{
    [System.Serializable]
    public class SaveData
    {
        public int coins;
        public string lastProfile = "default";
    }
}

namespace hp55games.Mobile.Core.Architecture
{
    public interface ISaveService
    {
        hp55games.Mobile.Core.Save.SaveData Data { get; }
        void Load();
        void Save();
    }

    public sealed class JsonSaveService : ISaveService
    {
        private const string FileName = "save.json";
        public hp55games.Mobile.Core.Save.SaveData Data { get; private set; } = new();

        public void Load()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
                Data = JsonUtility.FromJson<hp55games.Mobile.Core.Save.SaveData>(File.ReadAllText(path));
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

