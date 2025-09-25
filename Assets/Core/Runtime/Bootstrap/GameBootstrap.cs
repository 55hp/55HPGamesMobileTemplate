using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Architecture.States;
using hp55games.Mobile.Core.Runtime.Util;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace hp55games.Mobile.Core.Bootstrap
{
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
            ServiceRegistry.InstallDefaults();
            await SceneManager.LoadSceneAsync("Scenes/Additive/UI_Root", LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync("Scenes/Additive/Systems_Audio", LoadSceneMode.Additive);
            GameStateMachine.Instance.SetState(new MainMenuState());
        }
    }
}