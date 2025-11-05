using System;

namespace hp55games.Mobile.Core.UI
{
    public interface IOptionsService {
        float MusicVolume { get; set; }
        float SfxVolume { get; set; }
        bool  Haptics    { get; set; }
        event Action Changed;
        void Load();  void Save();
    }
}


