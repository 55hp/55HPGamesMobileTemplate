using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.Architecture;

namespace hp55games.Mobile.Core.Bootstrap
{
    /// <summary>
    /// Entry point:
    /// - installa i servizi core
    /// - carica 90_Systems_Audio, 91_UI_Root, 01_Menu in additive
    /// - imposta 01_Menu come ActiveScene
    /// 
    /// La FSM NON parte da qui: viene avviata da InitialStateInstaller nella 01_Menu.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void Init()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        private async void Awake()
        {
            DontDestroyOnLoad(gameObject);

            // 1) Servizi core (IGameStateMachine compreso)
            ServiceRegistry.InstallDefaults();

            // 2) Systems Audio
            await LoadSceneAdditiveAsync("Scenes/Additive/90_Systems_Audio");

            // 3) UI Root
            await LoadSceneAdditiveAsync("Scenes/Additive/91_UI_Root");

            // 4) Scena di menu
            await LoadSceneAdditiveAsync("Scenes/01_Menu");

            // Imposta 01_Menu come Active Scene
            var menuScene = SceneManager.GetSceneByPath("Scenes/01_Menu");
            if (menuScene.IsValid())
                SceneManager.SetActiveScene(menuScene);
        }

        private static async Task LoadSceneAdditiveAsync(string scenePath)
        {
            if (SceneManager.GetSceneByPath(scenePath).isLoaded)
                return;

            var op = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            if (op == null)
            {
                Debug.LogError("[GameBootstrap] Failed to load scene: " + scenePath);
                return;
            }

            while (!op.isDone)
            {
                await Task.Yield();
            }
        }
    }
}
