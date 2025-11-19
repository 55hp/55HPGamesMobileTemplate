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

        private void Awake()
        {
            _label = GetComponent<TextMeshProUGUI>();

            if (!ServiceRegistry.TryResolve<ILocalizationService>(out _loc))
            {
                Debug.LogError("[UILocalizedText] ILocalizationService not found.");
                enabled = false;
                return;
            }

            // Ci iscriviamo al cambio lingua
            _loc.LanguageChanged += OnLanguageChanged;

            // Aggiorniamo subito alla lingua corrente
            Refresh();
        }

        private void OnDestroy()
        {
            if (_loc != null)
                _loc.LanguageChanged -= OnLanguageChanged;
        }

        private void OnLanguageChanged()
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_label == null || _loc == null || string.IsNullOrEmpty(localizationKey))
                return;

            _label.text = _loc.Get(localizationKey);
        }

        /// <summary>
        /// Facoltativo: permette di cambiare la chiave da codice.
        /// </summary>
        public void SetKey(string key)
        {
            localizationKey = key;
            Refresh();
        }
    }
}