using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Core.AppLifecycle;
using hp55games.Mobile.Core.Save;

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

            // 1.5) Unity Gaming Services — async and fallible, so it can't live inside
            // InstallDefaults() (which every other registration depends on staying
            // synchronous). See InitializeUgsCoroutine's doc comment and the dossier Fase 2
            // commit message for the full rationale.
            yield return InitializeUgsCoroutine();

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

        /// <summary>
        /// Initializes Unity Gaming Services and, only on success, signs in anonymously and
        /// registers IAuthenticationService/ICloudSaveService. Deliberately NOT part of
        /// ServiceRegistry.InstallDefaults(): Unity.Services.Core.UnityServices.InitializeAsync()
        /// is async and can fail (no network, UGS project not linked for this Unity project),
        /// and InstallDefaults() must stay synchronous for every one of its other
        /// registrations — adding one fallible async registration there would put all of them
        /// at risk, not just this one.
        ///
        /// On failure this logs via ILog and leaves both services unregistered rather than
        /// registering a broken instance — callers must resolve these two specifically with
        /// ServiceRegistry.TryResolve&lt;T&gt;(), not Resolve&lt;T&gt;(), since unlike every
        /// other default service they may legitimately be absent (see IAuthenticationService
        /// and ICloudSaveService's own doc comments).
        ///
        /// Wrapped as a coroutine by polling Task.IsCompleted, the same way
        /// LoadSceneAdditiveCoroutine below polls AsyncOperation.isDone — this file's existing
        /// pattern for turning an async-style operation into something BootstrapSequence can
        /// yield on, applied here to System.Threading.Tasks.Task instead of Unity's own
        /// AsyncOperation. Unity.Services.* namespaces are deliberately referenced fully
        /// qualified rather than via "using" directives: Unity.Services.Authentication and
        /// Unity.Services.CloudSave each declare their own IAuthenticationService/
        /// ICloudSaveService interfaces, identically named to this template's — importing
        /// either unqualified here would make every reference to hp55games.Mobile.Core.Save's
        /// versions ambiguous (CS0104), the same collision shape UGSAuthenticationService and
        /// UGSCloudSaveService avoid for the same reason.
        /// </summary>
        private static IEnumerator InitializeUgsCoroutine()
        {
            var log = ServiceRegistry.Resolve<ILog>();

            var initTask = Unity.Services.Core.UnityServices.InitializeAsync();
            while (!initTask.IsCompleted)
                yield return null;

            if (initTask.IsFaulted)
            {
                log.Error("[GameBootstrap] UnityServices.InitializeAsync failed — " +
                          "IAuthenticationService/ICloudSaveService will remain unregistered. " +
                          initTask.Exception?.GetBaseException().Message);
                yield break;
            }

            var auth = new UGSAuthenticationService();
            var signInTask = auth.SignInAnonymouslyAsync();
            while (!signInTask.IsCompleted)
                yield return null;

            if (signInTask.IsFaulted || !signInTask.Result)
            {
                log.Error("[GameBootstrap] UGS anonymous sign-in failed — " +
                          "IAuthenticationService/ICloudSaveService will remain unregistered.");
                yield break;
            }

            ServiceRegistry.Register<IAuthenticationService>(auth);
            ServiceRegistry.Register<ICloudSaveService>(new UGSCloudSaveService());
            log.Info("[GameBootstrap] UGS Authentication + Cloud Save registered.");
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