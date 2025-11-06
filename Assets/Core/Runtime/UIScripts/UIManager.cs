using UnityEngine;
using hp55games.Mobile.Core.Architecture;

namespace hp55games.Ui
{
    /// <summary>
    /// Entry-point UI atteso dall'audit. Fornisce metodi semplici per Popup/Toast/Loading.
    /// Si appoggia ai tuoi componenti: UIRoot, UIPopup_Generic, UIToast, UILoadingOverlay.
    /// </summary>
    public static class UiManager
    {
        static UIRoot _root;
        static UIPopup_Generic _popup;
        static UIToast _toast;
        static UILoadingOverlay _loading;
        private static ILog cachedLogger;

        public static void Initialize()
        {
            // Trova (una volta) i componenti nella scena UI_Root caricata in additive
            _root    = Object.FindFirstObjectByType<UIRoot>();
            _popup   = Object.FindFirstObjectByType<UIPopup_Generic>(FindObjectsInactive.Include);
            _toast   = Object.FindFirstObjectByType<UIToast>(FindObjectsInactive.Include);
            _loading = Object.FindFirstObjectByType<UILoadingOverlay>(FindObjectsInactive.Include);

            cachedLogger = ServiceRegistry.Resolve<ILog>();
            cachedLogger?.Info("[UiManager] Initialized");
        }

        public static void ShowPopup(string title, string body, System.Action onConfirm = null, System.Action onCancel = null)
        {
            if (_popup == null) { cachedLogger?.Warn("UIPopup_Generic not found"); return; }
            _popup.Open(title, body, onConfirm, onCancel);
        }

        public static void ShowToast(string message, float? durationSeconds = null)
        {
            if (_toast == null) { cachedLogger?.Warn("UIToast not found"); return; }
            if (durationSeconds.HasValue) _toast.duration = durationSeconds.Value;
            _toast.Show(message);
        }

        public static void SetLoading(bool visible)
        {
            if (_loading == null) { cachedLogger?.Warn("UILoadingOverlay not found"); return; }
            if (visible) _loading.Show(); else _loading.Hide();
        }
    }
}