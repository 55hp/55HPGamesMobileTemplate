using System.Threading.Tasks;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Architecture.States;
using hp55games.Mobile.Core.Config;
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

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            ServiceRegistry.InstallDefaults();

            // NIENTE async void: lanciamo la sequenza asincrona in modo sicuro
            BootSequenceAsync().SafeFireAndForget();
        }

        private async Task BootSequenceAsync()
        {
            // 1) Config pronta
            await ServiceRegistry.Resolve<IConfigService>().InitializeAsync();

            // 2) Carica la scena principale del menu (Single)
            await SceneManager.LoadSceneAsync("01_Menu", LoadSceneMode.Single);
            //    (Assicurati che "01_Menu" sia in Build Settings)

            // 3) Carica i sistemi condivisi come additive
            await SceneManager.LoadSceneAsync("Scenes/Additive/91_UI_Root", LoadSceneMode.Additive);
            await SceneManager.LoadSceneAsync("Scenes/Additive/90_Systems_Audio", LoadSceneMode.Additive);

            // 4) NIENTE cambio di stato qui: lo fa l'InitialStateInstaller dentro 01_Menu
        }
    }
}