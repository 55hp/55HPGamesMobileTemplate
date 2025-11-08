using UnityEngine;
using UnityEngine.UI;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;
using TMPro;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Pannello Opzioni: collega slider/toggle/dropdown al servizio centrale.
    /// Chiama opt.Load() all'avvio, salva su cambi.
    /// </summary>
    public sealed class UIOptionsPage : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider musicSlider;   // 0..1
        [SerializeField] private Slider sfxSlider;     // 0..1
        [SerializeField] private Toggle hapticsToggle;
        [SerializeField] private TMP_Dropdown languageDropdown; // valori: "en","it",...

        [Header("Optional")]
        [SerializeField] private Button applyButton;   // Se nullo, salva on-change
        [SerializeField] private Button resetButton;   // Ripristina default
        [SerializeField] private Toggle musicMuteToggle;
        [SerializeField] private Toggle sfxMuteToggle;
        
        private IUIOptionsService _opt;

        void Awake()
        {
            _opt = ServiceRegistry.Resolve<IUIOptionsService>();
            _opt.Load(); // assicura stato pronto

            // wire UI -> model
            if (applyButton != null)
            {
                applyButton.onClick.AddListener(ApplyFromUI);
            }
            else
            {
                if (musicSlider)   musicSlider.onValueChanged.AddListener(_ => ApplyFromUI());
                if (sfxSlider)     sfxSlider.onValueChanged.AddListener(_ => ApplyFromUI());
                if (hapticsToggle) hapticsToggle.onValueChanged.AddListener(_ => ApplyFromUI());
                if (languageDropdown) languageDropdown.onValueChanged.AddListener(_ => ApplyFromUI());
                if (musicMuteToggle) musicMuteToggle.onValueChanged.AddListener(_ => ApplyFromUI());
                if (sfxMuteToggle)   sfxMuteToggle.onValueChanged.AddListener(_ => ApplyFromUI());
            }

            if (resetButton) resetButton.onClick.AddListener(() =>
            {
                _opt.ResetToDefaults();
                _opt.Save();
                RefreshUIFromModel();
            });
        }

        void OnEnable()
        {
            _opt.Changed += RefreshUIFromModel;
            RefreshUIFromModel();
        }

        void OnDisable()
        {
            _opt.Changed -= RefreshUIFromModel;
        }

        private void RefreshUIFromModel()
        {
            if (musicSlider)   musicSlider.SetValueWithoutNotify(_opt.MusicVolume);
            if (sfxSlider)     sfxSlider.SetValueWithoutNotify(_opt.SfxVolume);
            if (hapticsToggle) hapticsToggle.SetIsOnWithoutNotify(_opt.Haptics);
            if (musicMuteToggle) musicMuteToggle.SetIsOnWithoutNotify(_opt.MusicMute);
            if (sfxMuteToggle)   sfxMuteToggle.SetIsOnWithoutNotify(_opt.SfxMute);
            
            if (languageDropdown)
            {
                var lang = _opt.Language ?? "en";
                int idx = languageDropdown.options.FindIndex(o => o.text == lang);
                if (idx < 0) idx = 0;
                languageDropdown.SetValueWithoutNotify(idx);
            }
        }

        private void ApplyFromUI()
        {
            if (musicSlider)   _opt.MusicVolume = musicSlider.value;
            if (sfxSlider)     _opt.SfxVolume   = sfxSlider.value;
            if (hapticsToggle) _opt.Haptics     = hapticsToggle.isOn;
            if (musicMuteToggle) _opt.MusicMute = musicMuteToggle.isOn;
            if (sfxMuteToggle)   _opt.SfxMute   = sfxMuteToggle.isOn;

            if (languageDropdown)
            {
                var txt = languageDropdown.options[languageDropdown.value].text;
                _opt.Language = string.IsNullOrEmpty(txt) ? "en" : txt;
            }

            _opt.Save();
        }
    }
}
