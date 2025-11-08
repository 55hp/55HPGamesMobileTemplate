// Assets/Core/Runtime/Options/UIOptionsService.cs
using System;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SaveService; // per SaveData/OptionsData

namespace hp55games.Mobile.Core.UI
{
    /// <summary>Servizio centrale per le opzioni: legge/scrive SaveService.Data.options</summary>
    public sealed class UIOptionsService : IUIOptionsService
    {
        public event Action Changed;

        public float MusicVolume { get => _music; set { _music = Clamp01(value); Changed?.Invoke(); } }
        public float SfxVolume   { get => _sfx;   set { _sfx   = Clamp01(value); Changed?.Invoke(); } }
        public bool  Haptics     { get => _hapt;  set { _hapt  = value;          Changed?.Invoke(); } }
        public string Language   { get => _lang;  set { _lang  = value ?? "en";  Changed?.Invoke(); } }
        public bool  MusicMute   { get => _musicMute; set { _musicMute = value; Changed?.Invoke(); } }
        public bool  SfxMute     { get => _sfxMute;   set { _sfxMute   = value; Changed?.Invoke(); } }
        
        float _music = 0.8f;
        float _sfx   = 0.8f;
        bool  _hapt  = true;
        string _lang = "en";
        bool _musicMute = false, _sfxMute = false;

        readonly ISaveService _save;

        public UIOptionsService()
        {
            _save = ServiceRegistry.Resolve<ISaveService>();
        }

        public void Load()
        {
            // Assicura che SaveService abbia caricato i dati
            _save.Load();

            // leggi dal blocco options
            OptionsData opt = _save.Data.options ?? new OptionsData();
            _music = Clamp01(opt.music);
            _sfx   = Clamp01(opt.sfx);
            _hapt  = opt.hapt;
            _lang  = string.IsNullOrEmpty(opt.lang) ? "en" : opt.lang;
            
            _musicMute = opt.musicMute;
            _sfxMute   = opt.sfxMute;
            
            // fallback ai default da GameConfig (solo se vuoi)
            if (ServiceRegistry.TryResolve<IConfigService>(out var cfg) && cfg.Current != null)
            {
                _hapt = cfg.Current.enableHaptics;
            }

            Changed?.Invoke();
        }

        public void Save()
        {
            var opt = _save.Data.options ?? new OptionsData();
            opt.music = _music;
            opt.sfx   = _sfx;
            opt.hapt  = _hapt;
            opt.lang  = _lang;
            opt.musicMute = _musicMute; opt.sfxMute = _sfxMute;
            
            _save.Data.options = opt;
            _save.Save();
        }

        public void ResetToDefaults()
        {
            _music = 0.8f; _sfx = 0.8f; _hapt = true; _lang = "en";
            _musicMute = false; _sfxMute = false;
            Changed?.Invoke();
        }

        static float Clamp01(float v) => v < 0 ? 0 : (v > 1 ? 1 : v);
    }
}
