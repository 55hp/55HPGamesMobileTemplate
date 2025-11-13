using System;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SaveService;

namespace hp55games.Mobile.Core.UI
{
    /// <summary>
    /// Servizio centrale per le opzioni.
    /// Legge/scrive i dati in SaveService.Data.options.
    /// Espone eventi per notificare la UI e i binder (es. audio).
    /// </summary>
    public sealed class UIOptionsService : IUIOptionsService
    {
        public event Action Changed;

        public float MusicVolume
        {
            get => _music;
            set { _music = Clamp01(value); Changed?.Invoke(); }
        }

        public float SfxVolume
        {
            get => _sfx;
            set { _sfx = Clamp01(value); Changed?.Invoke(); }
        }

        public bool Haptics
        {
            get => _hapt;
            set { _hapt = value; Changed?.Invoke(); }
        }

        public string Language
        {
            get => _lang;
            set { _lang = string.IsNullOrEmpty(value) ? "en" : value; Changed?.Invoke(); }
        }

        public bool MusicMute
        {
            get => _musicMute;
            set { _musicMute = value; Changed?.Invoke(); }
        }

        public bool SfxMute
        {
            get => _sfxMute;
            set { _sfxMute = value; Changed?.Invoke(); }
        }

        float _music = 0.8f;
        float _sfx   = 0.8f;
        bool  _hapt  = true;
        string _lang = "en";
        bool _musicMute = false;
        bool _sfxMute   = false;

        readonly ISaveService _save;

        public UIOptionsService()
        {
            _save = ServiceRegistry.Resolve<ISaveService>();
        }

        public void Load()
        {
            // Carica il save globale
            _save.Load();

            // Assicura che il blocco options non sia null
            var opt = _save.Data.options ?? new OptionsData();
            _save.Data.options ??= opt;

            // Legge i valori
            _music     = Clamp01(opt.music);
            _sfx       = Clamp01(opt.sfx);
            _hapt      = opt.hapt;
            _lang      = string.IsNullOrEmpty(opt.lang) ? "en" : opt.lang;
            _musicMute = opt.musicMute;
            _sfxMute   = opt.sfxMute;

            // Fallback config per gli haptics
            if (ServiceRegistry.TryResolve<IConfigService>(out var cfg) && cfg.Current != null)
            {
                _hapt = cfg.Current.enableHaptics;
            }

            Changed?.Invoke();
        }

        public void Save()
        {
            var opt = _save.Data.options ?? new OptionsData();
            opt.music     = _music;
            opt.sfx       = _sfx;
            opt.hapt      = _hapt;
            opt.lang      = _lang;
            opt.musicMute = _musicMute;
            opt.sfxMute   = _sfxMute;

            _save.Data.options = opt;
            _save.Save();
        }

        public void ResetToDefaults()
        {
            _music     = 0.8f;
            _sfx       = 0.8f;
            _hapt      = true;
            _lang      = "en";
            _musicMute = false;
            _sfxMute   = false;

            Changed?.Invoke();
        }

        static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}
