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
        // Adjust these paths if your scene paths differ.
        private const string MenuScenePath     = "Scenes/01_Menu";
        private const string GameplayScenePath = "Scenes/02_Gameplay";
        private const string ResultsScenePath  = "Scenes/03_Results";

        // Default fade duration for transitions
        private const float FadeDuration = 0.25f;

        private readonly IGameStateMachine _fsm;
        private readonly IUIOverlayService _overlay;

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

        public async Task GoToMenuAsync()
        {
            Debug.Log("[SceneFlowService] GoToMenuAsync()");

            await RunWithOverlay(async () =>
            {
                await EnsureSceneLoadedAndActive(MenuScenePath);

                if (_fsm != null)
                {
                    Debug.Log("[SceneFlowService] Changing FSM state to MainMenuState.");
                    await _fsm.ChangeStateAsync(new MainMenuState());
                }
                else
                {
                    Debug.LogWarning("[SceneFlowService] FSM is null. Only scene changed, no state transition.");
                }
            });
        }

        public async Task GoToGameplayAsync(string levelId = null)
        {
            Debug.Log($"[SceneFlowService] GoToGameplayAsync(levelId: {levelId})");

            await RunWithOverlay(async () =>
            {
                await EnsureSceneLoadedAndActive(GameplayScenePath);

                if (_fsm != null)
                {
                    // TODO: wire a proper GameplayState when available
                    Debug.LogWarning("[SceneFlowService] No GameplayState wired yet. Only scene changed.");
                }
            });
        }

        public async Task GoToResultsAsync()
        {
            Debug.Log("[SceneFlowService] GoToResultsAsync()");

            await RunWithOverlay(async () =>
            {
                await EnsureSceneLoadedAndActive(ResultsScenePath);

                if (_fsm != null)
                {
                    // TODO: wire a proper ResultsState when available
                    Debug.LogWarning("[SceneFlowService] No ResultsState wired yet. Only scene changed.");
                }
            });
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

        /// <summary>
        /// Ensures the given scene is loaded additively.
        /// If not loaded, loads it additively and waits for completion.
        /// Then sets it as active scene.
        /// </summary>
        private static async Task EnsureSceneLoadedAndActive(string scenePath)
        {
            if (string.IsNullOrWhiteSpace(scenePath))
            {
                Debug.LogError("[SceneFlowService] Scene path is null or empty.");
                return;
            }

            var scene = SceneManager.GetSceneByPath(scenePath);

            if (!scene.isLoaded)
            {
                Debug.Log($"[SceneFlowService] Loading scene additively: {scenePath}");
                var op = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
                if (op == null)
                {
                    Debug.LogError($"[SceneFlowService] LoadSceneAsync returned null for: {scenePath}");
                    return;
                }

                while (!op.isDone)
                    await Task.Yield();

                scene = SceneManager.GetSceneByPath(scenePath);
            }

            if (!scene.IsValid())
            {
                Debug.LogError($"[SceneFlowService] Scene not valid after load: {scenePath}");
                return;
            }

            SceneManager.SetActiveScene(scene);
            Debug.Log($"[SceneFlowService] Active scene set to: {scenePath}");
        }
    }
}
