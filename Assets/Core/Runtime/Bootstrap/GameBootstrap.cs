using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Core.AppLifecycle;

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
            gameObject.AddComponent<AppLifecycleHandler>();
            StartCoroutine(BootstrapSequence());
        }

        private IEnumerator BootstrapSequence()
        {
            // 1) Core services (Save, Time, EventBus, etc.)
            ServiceRegistry.InstallDefaults();

            // 2) SceneFlowConfig — must be registered before any scene with a
            //    SceneFlowServiceInstaller loads, so SceneFlowService can resolve it.
            var configHandle = Addressables.LoadAssetAsync<SceneFlowConfig>(
                hp55games.Addr.Config.SceneFlowConfig);
            yield return configHandle;

            if (configHandle.Status == AsyncOperationStatus.Succeeded)
            {
                ServiceRegistry.Register<ISceneFlowConfig>(configHandle.Result);
                Debug.Log("[GameBootstrap] SceneFlowConfig registered.");
            }
            else
            {
                Debug.LogError("[GameBootstrap] Failed to load SceneFlowConfig. " +
                               "Scene flow will fall back to hardcoded scene names.");
            }

            // 3) Systems Audio
            yield return LoadSceneAdditiveCoroutine("Assets/Scenes/Additive/90_Systems_Audio.unity");

            // 4) UI Root
            yield return LoadSceneAdditiveCoroutine("Assets/Scenes/Additive/91_UI_Root.unity");

            // 5) Menu scene
            yield return LoadSceneAdditiveCoroutine("Assets/Scenes/01_Menu.unity");

            var menuScene = SceneManager.GetSceneByPath("Assets/Scenes/01_Menu.unity");
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