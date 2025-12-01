using UnityEngine;
using TMPro;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Localization;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Collega un TextMeshProUGUI a una chiave di localizzazione.
    /// Usa ILocalizationService e si aggiorna quando cambia la lingua.
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class UILocalizedText : MonoBehaviour
    {
        [SerializeField]
        private string localizationKey;
        
        private TextMeshProUGUI _label;
        private ILocalizationService _loc;
        private string prefix = "";
        private string suffix = "";
        
        private void Awake()
        {
            _label = GetComponent<TextMeshProUGUI>();

            if (!ServiceRegistry.TryResolve<ILocalizationService>(out _loc))
            {
                Debug.LogError("[UILocalizedText] ILocalizationService not found.");
                enabled = false;
                return;
            }

            _loc.LanguageChanged += Refresh;
            
            Refresh();
        }

        public void Refresh()
        {
            if (_label == null || _loc == null || string.IsNullOrEmpty(localizationKey))
                return;

            _label.text = prefix + _loc.Get(localizationKey) + suffix;
        }

        public void Setprefix(string prefix)
        {
            this.prefix = prefix;
        }

        public void SetSuffix(string suffix)
        {
            this.suffix = suffix;
        }
    }
}