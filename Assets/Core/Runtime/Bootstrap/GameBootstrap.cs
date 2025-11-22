using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.Architecture;

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
            StartCoroutine(BootstrapSequence());
        }

        private IEnumerator BootstrapSequence()
        {
            // 1) Servizi core (compreso Save + Time)
            ServiceRegistry.InstallDefaults();

            // 2) Systems Audio
            yield return LoadSceneAdditiveCoroutine("Scenes/Additive/90_Systems_Audio");

            // 3) UI Root
            yield return LoadSceneAdditiveCoroutine("Scenes/Additive/91_UI_Root");

            // 4) Scena di menu
            yield return LoadSceneAdditiveCoroutine("Scenes/01_Menu");

            var menuScene = SceneManager.GetSceneByPath("Scenes/01_Menu");
            if (menuScene.IsValid())
                SceneManager.SetActiveScene(menuScene);
        }

        private static IEnumerator LoadSceneAdditiveCoroutine(string scenePath)
        {
            if (SceneManager.GetSceneByPath(scenePath).isLoaded)
                yield break;

            var op = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
            if (op == null)
            {
                Debug.LogError("[GameBootstrap] Failed to load scene: " + scenePath);
                yield break;
            }

            while (!op.isDone)
                yield return null;
        }
    }
}