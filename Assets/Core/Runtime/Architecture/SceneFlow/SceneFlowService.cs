using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Architecture.States;
using hp55games.Mobile.Core.UI; // Assumes IUIOverlayService is here

namespace hp55games.Mobile.Game.SceneFlow
{
    /// <summary>
    /// High-level scene flow orchestration.
    /// 
    /// Responsibilities:
    /// - Ensure Menu / Gameplay / Results scenes are loaded additively
    /// - Set the active scene
    /// - Integrate with FSM for MainMenuState
    /// - Integrate with UI Overlay for fade-in / fade-out
    /// 
    /// Next steps (future):
    /// - Smarter unload logic
    /// - Dedicated GameplayState / ResultsState wiring
    /// </summary>
    public sealed class SceneFlowService : ISceneFlowService
    {
        //TODO update this script with log service
        private const string MenuSceneName     = "01_Menu";
        private const string GameplaySceneName = "02_Gameplay";
        private const string ResultsSceneName  = "03_Results";

        // Default fade duration for transitions
        private const float FadeDuration = 0.25f;

        private readonly IGameStateMachine _fsm;
        private readonly IUIOverlayService _overlay;

        private static readonly string[] ContentScenes =
        {
            MenuSceneName,
            GameplaySceneName,
            ResultsSceneName
        };

        private async Task SwitchContentSceneAsync(string targetScene)
        {
            Debug.Log($"[SceneFlowService] Switching content scene to: {targetScene}");

            // 1) Unload all other content scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);

                if (!s.isLoaded)
                    continue;

                bool isContent = Array.IndexOf(ContentScenes, s.name) >= 0;
                if (!isContent)
                    continue;

                if (s.name == targetScene)
                    continue;

                Debug.Log($"[SceneFlowService] Unloading previous content scene: {s.name}");

                var op = SceneManager.UnloadSceneAsync(s);
                if (op != null)
                {
                    while (!op.isDone)
                        await Task.Yield(); // IMPORTANTISSIMO
                }
            }

            // 2) Load the target scene if not already loaded
            var target = SceneManager.GetSceneByName(targetScene);

            if (!target.isLoaded)
            {
                Debug.Log($"[SceneFlowService] Loading content scene additively: {targetScene}");

                var op = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
                if (op != null)
                {
                    while (!op.isDone)
                        await Task.Yield();
                }

                target = SceneManager.GetSceneByName(targetScene);
            }

            // 3) Set active scene
            if (target.IsValid())
            {
                SceneManager.SetActiveScene(target);
                Debug.Log($"[SceneFlowService] Active content scene set to: {targetScene}");
            }
            else
            {
                Debug.LogError($"[SceneFlowService] Loaded target scene {targetScene} is NOT valid!");
            }
        }
        
        public SceneFlowService()
        {
            if (!ServiceRegistry.TryResolve<IGameStateMachine>(out _fsm))
            {
                Debug.LogWarning("[SceneFlowService] IGameStateMachine not available. FSM integration will be disabled.");
            }

            if (!ServiceRegistry.TryResolve<IUIOverlayService>(out _overlay))
            {
                Debug.LogWarning("[SceneFlowService] IUIOverlayService not available. Overlay transitions will be skipped.");
            }
        }
        
        private async Task UnloadSceneIfLoadedAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return;

            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded)
                return;

            Debug.Log($"[SceneFlowService] Unloading scene: {sceneName}");
            var op = SceneManager.UnloadSceneAsync(scene);

            if (op == null)
                return;

            while (!op.isDone)
                await Task.Yield();
        }

        private async Task LoadSceneIfNotLoadedAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return;

            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
                return;

            Debug.Log($"[SceneFlowService] Loading scene additively: {sceneName}");
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            if (op == null)
                return;

            while (!op.isDone)
                await Task.Yield();
        }


        public async Task GoToMenuAsync()
        {
            Debug.Log("[SceneFlowService] GoToMenuAsync()");

            await RunWithOverlay(async () =>
            {
                // 1) Non vogliamo né Gameplay né Results quando siamo nel menu
                await UnloadSceneIfLoadedAsync(GameplaySceneName);
                await UnloadSceneIfLoadedAsync(ResultsSceneName);

                // 2) Il menu deve essere sempre disponibile
                await LoadSceneIfNotLoadedAsync(MenuSceneName);

                var menuScene = SceneManager.GetSceneByName(MenuSceneName);
                if (menuScene.IsValid())
                {
                    SceneManager.SetActiveScene(menuScene);
                }

                // 3) Stato FSM
                if (_fsm != null)
                {
                    Debug.Log("[SceneFlowService] Changing FSM state to MainMenuState.");
                    await _fsm.ChangeStateAsync(new MainMenuState());
                }
                else
                {
                    Debug.LogWarning("[SceneFlowService] FSM is null in GoToMenuAsync. Only scene changed.");
                }
            });
        }

        
        private void DebugLoadedScenes(string context)
        {
            Debug.Log($"[SceneFlowService] --- Loaded scenes ({context}) ---");

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var s = SceneManager.GetSceneAt(i);
                Debug.Log($"[SceneFlowService]   {i}: {s.name} (loaded={s.isLoaded}, active={s == SceneManager.GetActiveScene()})");
            }
        }

        public async Task GoToGameplayAsync(string levelId = null)
        {
            Debug.Log($"[SceneFlowService] GoToGameplayAsync(levelId: {levelId})");

            await RunWithOverlay(async () =>
            {
                // 1) In gameplay non vogliamo la scena di Results
                await UnloadSceneIfLoadedAsync(ResultsSceneName);

                // 2) Assicuriamoci che il gameplay sia caricato
                await LoadSceneIfNotLoadedAsync(GameplaySceneName);

                var gameplayScene = SceneManager.GetSceneByName(GameplaySceneName);
                if (gameplayScene.IsValid())
                {
                    SceneManager.SetActiveScene(gameplayScene);
                }

                // 3) Stato FSM
                if (_fsm != null)
                {
                    Debug.Log("[SceneFlowService] Changing FSM state to GameplayState.");
                    await _fsm.ChangeStateAsync(new GameplayState());
                }
                else
                {
                    Debug.LogWarning("[SceneFlowService] FSM is null in GoToGameplayAsync. Only scene changed.");
                }
            });
        }
 

        public async Task GoToResultsAsync()
        {
            Debug.Log("[SceneFlowService] GoToResultsAsync()");

            await RunWithOverlay(async () =>
            {
                // 1) In results non ci serve più il gameplay
                await UnloadSceneIfLoadedAsync(GameplaySceneName);

                // 2) Carichiamo la scena Results se necessario
                await LoadSceneIfNotLoadedAsync(ResultsSceneName);

                var resultsScene = SceneManager.GetSceneByName(ResultsSceneName);
                if (resultsScene.IsValid())
                {
                    SceneManager.SetActiveScene(resultsScene);
                }

                // 3) Stato FSM
                if (_fsm != null)
                {
                    Debug.Log("[SceneFlowService] Changing FSM state to ResultState.");
                    await _fsm.ChangeStateAsync(new ResultState());
                }
                else
                {
                    Debug.LogWarning("[SceneFlowService] FSM is null in GoToResultsAsync. Only scene changed.");
                }
            });
        }

        
        public async Task GoToPauseAsync()
        {
            if (_fsm == null)
            {
                //TODO update this script with log service _log?.Warn("[SceneFlowService] GoToPauseAsync called but state machine is null.");
                return;
            }

            await _fsm.ChangeStateAsync(new PauseState());
        }

        /// <summary>
        /// Helper: wraps an async scene transition with optional fade-in/out.
        /// If overlay is not available, simply executes the action.
        /// </summary>
        private async Task RunWithOverlay(Func<Task> action)
        {
            if (_overlay != null)
            {
                await _overlay.FadeInAsync(FadeDuration);
            }

            await action();

            if (_overlay != null)
            {
                await _overlay.FadeOutAsync(FadeDuration);
            }
        }
    }
}
