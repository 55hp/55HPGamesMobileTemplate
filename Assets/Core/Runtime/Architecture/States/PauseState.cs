using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Architecture.States
{
    /// <summary>
    /// Generic pause state:
    /// - Freezes gameplay time (Time.timeScale = 0).
    /// - Opens a generic pause popup via IUIPopupService.
    /// - On exit, restores previous timeScale and closes the popup.
    /// </summary>
    public sealed class PauseState : IGameState
    {
        private const string PausePopupAddress = Addr.Content.UI.Popups.Popup_Pause;

        private float _previousTimeScale;
        private GameObject _pausePopup;

        public async Task EnterAsync(CancellationToken ct)
        {
            Debug.Log("[PauseState] Enter");

            _previousTimeScale = UnityEngine.Time.timeScale;
            UnityEngine.Time.timeScale = 0f;

            var popupService = ServiceRegistry.Resolve<IUIPopupService>();

            try
            {
                // Apri il popup di pausa. In Step 3 collegheremo i pulsanti a Resume/Leave.
                _pausePopup = await popupService.OpenAsync(PausePopupAddress);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[PauseState] Failed to open pause popup at '{PausePopupAddress}': {ex}");
            }
        }

        public Task ExitAsync(CancellationToken ct)
        {
            Debug.Log("[PauseState] Exit");

            // Ripristina il tempo di gioco
            UnityEngine.Time.timeScale = _previousTimeScale;

            // Chiudi il popup se ancora aperto
            if (_pausePopup != null)
            {
                var popupService = ServiceRegistry.Resolve<IUIPopupService>();
                popupService.Close(_pausePopup);
                _pausePopup = null;
            }

            return Task.CompletedTask;
        }
    }
}