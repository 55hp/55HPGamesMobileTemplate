using UnityEngine;
using UnityEngine.UI;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Game.UI
{
    /// <summary>
    /// Generic Pause popup controller.
    /// - Resume: exits PauseState by going back to Gameplay via SceneFlow.
    /// - Options: navigates to the generic options page.
    /// - Leave: triggers end-of-game flow (Results) via SceneFlow.
    /// </summary>
    public sealed class UIPopup_Pause : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _optionsButton;
        [SerializeField] private Button _leaveButton;

        private ISceneFlowService _sceneFlow;
        private IUINavigationService _navigation;
        private IUIPopupService _popupService;

        private void Awake()
        {
            _sceneFlow  = ServiceRegistry.Resolve<ISceneFlowService>();
            _navigation = ServiceRegistry.Resolve<IUINavigationService>();
            _popupService = ServiceRegistry.Resolve<IUIPopupService>();

            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(OnResumeClicked);

            if (_optionsButton != null)
                _optionsButton.onClick.AddListener(OnOptionsClicked);

            if (_leaveButton != null)
                _leaveButton.onClick.AddListener(OnLeaveClicked);
        }

        private void OnDestroy()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.RemoveListener(OnResumeClicked);

            if (_optionsButton != null)
                _optionsButton.onClick.RemoveListener(OnOptionsClicked);

            if (_leaveButton != null)
                _leaveButton.onClick.RemoveListener(OnLeaveClicked);
        }

        private void OnResumeClicked()
        {
            _popupService.Close(gameObject);
        }

        private async void OnOptionsClicked()
        {
            if (_navigation == null)
            {
                Debug.LogWarning("[UIPopup_Pause] IUINavigationService not found. Options will do nothing.");
                return;
            }

            // Usa l'indirizzo centralizzato in Addr (già presente nel template)
            await _navigation.PushAsync(hp55games.Addr.Content.UI.Pages.Options_Page);
        }

        private async void OnLeaveClicked()
        {
            if (_sceneFlow == null)
            {
                Debug.LogWarning("[UIPopup_Pause] ISceneFlowService not found. Leave will do nothing.");
                return;
            }

            // "Leave the game" = vai al flusso di fine partita (Results).
            await _sceneFlow.GoToResultsAsync();
        }
    }
}
