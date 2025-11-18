using UnityEngine;
using UnityEngine.UI;
using TMPro;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Pannello Opzioni: collega slider/toggle/dropdown al servizio centrale.
    /// Se c'è un applyButton, salva solo su click; altrimenti salva on-change.
    /// </summary>
    public sealed class UIOptionsPage : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Slider musicSlider;   // 0..1
        [SerializeField] private Slider sfxSlider;     // 0..1
        [SerializeField] private Toggle hapticsToggle;
        [SerializeField] private TMP_Dropdown languageDropdown; // valori: "en","it",...
        [SerializeField] private Button backButton;
        
        [Header("Optional")]
        [SerializeField] private Button applyButton;   // Se nullo, salva on-change
        [SerializeField] private Button resetButton;   // Ripristina default
        [SerializeField] private Toggle musicMuteToggle;
        [SerializeField] private Toggle sfxMuteToggle;

        private IUIOptionsService _opt;
        private IUINavigationService _navigation;
        private bool _isApplying;

        void Awake()
        {
            _opt = ServiceRegistry.Resolve<IUIOptionsService>();
            _opt.Load(); // assicura stato pronto
            
            if (!ServiceRegistry.TryResolve<IUINavigationService>(out _navigation))
            {
                Debug.LogWarning("[UIOptionsPage] IUINavigationService not available. Back button will do nothing.");
            }

            // wiring UI -> model
            if (applyButton != null)
            {
                applyButton.onClick.AddListener(ApplyFromUI);
            }
            else
            {
                if (musicSlider)      musicSlider.onValueChanged.AddListener(_ => ApplyFromUI());
                if (sfxSlider)        sfxSlider.onValueChanged.AddListener(_ => ApplyFromUI());
                if (hapticsToggle)    hapticsToggle.onValueChanged.AddListener(_ => ApplyFromUI());
                if (languageDropdown) languageDropdown.onValueChanged.AddListener(_ => ApplyFromUI());
                if (musicMuteToggle)  musicMuteToggle.onValueChanged.AddListener(_ => ApplyFromUI());
                if (sfxMuteToggle)    sfxMuteToggle.onValueChanged.AddListener(_ => ApplyFromUI());
            }

            if (resetButton)
            {
                resetButton.onClick.AddListener(() =>
                {
                    _opt.ResetToDefaults();
                    _opt.Save();
                    RefreshUIFromModel();
                });
            }
            
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }
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
            if (_isApplying) return; // evita loop mentre stiamo applicando

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
            _isApplying = true;
            try
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
                // refresh esplicito dopo il salvataggio
                RefreshUIFromModel();
            }
            finally
            {
                _isApplying = false;
            }
        }
        
        private async void OnBackClicked()
        {
            if (_navigation == null)
            {
                Debug.LogWarning("[UIOptionsPage] Back clicked but IUINavigationService is null.");
                return;
            }

            await _navigation.PopAsync();
        }
    }
}
