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

        [Header("Panels")]
        [SerializeField] private GameObject _optionsPanel;
        
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
            
            if(_optionsPanel != null)
                _optionsPanel.SetActive(false);
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

        private void OnOptionsClicked()
        {
            if (_optionsPanel == null)
            {
                Debug.LogWarning("[UIPopup_Pause] _optionsPanel not found or not referenced in the inspector. Options button will do nothing.");
                return;
            }

            _optionsPanel.SetActive(true);
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
