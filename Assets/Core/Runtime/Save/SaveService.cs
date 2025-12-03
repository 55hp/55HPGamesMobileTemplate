using System.IO;
using UnityEngine;

namespace hp55games.Mobile.Core.Architecture
{
    public sealed class SaveService : ISaveService
    {
        private const string FileName = "save.json";
        public hp55games.Mobile.Core.Save.SaveData Data { get; private set; } = new();

        public void Load()
        {
            var path = Path.Combine(Application.persistentDataPath, FileName);
            if (File.Exists(path))
            {
                Data = JsonUtility.FromJson<hp55games.Mobile.Core.Save.SaveData>(
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
