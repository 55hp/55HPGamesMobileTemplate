using System;

namespace hp55games.Mobile.Core.UI
{
    public interface IUIOptionsService
    {
        // Stato
        float MusicVolume { get; set; }   // 0..1
        float SfxVolume   { get; set; }   // 0..1
        bool  Haptics     { get; set; }
        string Language   { get; set; }   // "en", "it", ...
        bool  MusicMute   { get; set; }
        bool  SfxMute     { get; set; }
        
        event Action Changed;

        // Lifecycle
        void Load();
        void Save();
        void ResetToDefaults();
    }
}