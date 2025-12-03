namespace hp55games.Mobile.Core.Architecture
{

    public interface ISaveService
    {
        Save.SaveData Data { get; }
        void Load();
        void Save();
    }
}