using System.Threading.Tasks;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace hp55games.Mobile.Game.UI
{
    /// <summary>
    /// Main Menu UI controller.
    /// Wires buttons to high-level services:
    /// - Play -> ISceneFlowService.GoToGameplayAsync()
    /// - Options -> IUINavigationService.PushAsync(optionsPageAddress)
    /// - Credits -> IUINavigationService.PushAsync(creditsPageAddress)
    /// - Exit -> Application.Quit()
    /// 
    /// Addresses are plain strings so you can plug your Addressable keys
    /// (e.g. "content/ui/pages/options_page").
    /// </summary>
    public sealed class UIMainMenuPage : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button exitButton;

        private string optionsPageAddress = Addr.Content.UI.Pages.Options_Page;
        private string creditsPageAddress = Addr.Content.UI.Pages.Credits_Page;

        private ISceneFlowService _sceneFlow;
        private IUINavigationService _navigation;

        private void Awake()
        {
            if (!ServiceRegistry.TryResolve<ISceneFlowService>(out _sceneFlow))
            {
                Debug.LogWarning("[UIMainMenuPage] ISceneFlowService not available. Play button will do nothing.");
            }

            if (!ServiceRegistry.TryResolve<IUINavigationService>(out _navigation))
            {
                Debug.LogWarning("[UIMainMenuPage] IUINavigationService not available. Options/Credits will do nothing.");
            }

            // Wire buttons
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            if (optionsButton != null)
                optionsButton.onClick.AddListener(OnOptionsClicked);

            if (creditsButton != null)
                creditsButton.onClick.AddListener(OnCreditsClicked);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);
        }

        private async void OnPlayClicked()
        {
            if (_sceneFlow == null)
            {
                Debug.LogWarning("[UIMainMenuPage] Play clicked but ISceneFlowService is null.");
                return;
            }

            // 1) Chiudi la pagina del menu se hai un navigation service
            if (_navigation != null)
            {
                await _navigation.PopAsync();
            }

            // 2) Vai al gameplay (scene + state via SceneFlowService)
            await _sceneFlow.GoToGameplayAsync();
        }

        private async void OnOptionsClicked()
        {
            if (_navigation == null)
            {
                Debug.LogWarning("[UIMainMenuPage] Options clicked but IUINavigationService is null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(optionsPageAddress))
            {
                Debug.LogWarning("[UIMainMenuPage] Options clicked but optionsPageAddress is empty.");
                return;
            }

            await _navigation.PushAsync(optionsPageAddress);
        }

        private async void OnCreditsClicked()
        {
            ServiceRegistry.Resolve<hp55games.Mobile.Core.Localization.ILocalizationService>()
                .SetLanguage("it");
            
            if (_navigation == null)
            {
                Debug.LogWarning("[UIMainMenuPage] Credits clicked but IUINavigationService is null.");
                return;
            }

            if (string.IsNullOrWhiteSpace(creditsPageAddress))
            {
                Debug.LogWarning("[UIMainMenuPage] Credits clicked but creditsPageAddress is empty.");
                return;
            }
            
            await _navigation.PushAsync(creditsPageAddress);
        }

        private void OnExitClicked()
        {
            Debug.Log("[UIMainMenuPage] Exit clicked. Quitting application.");
            Application.Quit();

            // In Editor, Application.Quit() does nothing; the log at least shows it's wired.
        }
    }
}
