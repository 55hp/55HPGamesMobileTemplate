using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Architecture.States;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Core;

namespace hp55games.Mobile.Core.AppLifecycle
{
    /// <summary>
    /// Standard mobile app-lifecycle behavior. Added automatically by GameBootstrap onto the
    /// persistent bootstrap GameObject — no manual scene wiring needed.
    ///
    /// - Keeps the screen awake for the whole session (Screen.sleepTimeout = NeverSleep):
    ///   virtually every mobile game disables screen dimming/locking during play, not just
    ///   during active gameplay (menus need input too).
    /// - Auto-pauses an in-progress run when the app is backgrounded (OnApplicationPause /
    ///   OnApplicationFocus fire for the same event on different platforms, so both are
    ///   routed through the same guarded call). Only triggers from GameplayState: pausing
    ///   from Menu/Results/already-Paused would be meaningless, and SceneFlowService's own
    ///   "_isTransitioning" guard already no-ops a redundant GoToPauseAsync call.
    /// - Maps the Android hardware back button (Unity's legacy Input reports it as
    ///   KeyCode.Escape) to the platform-conventional action for the current state: open
    ///   Pause from Gameplay, Resume from Pause, quit the app from the Main Menu.
    /// </summary>
    public sealed class AppLifecycleHandler : MonoBehaviour
    {
        private void Awake()
        {
            // Set as early as possible; has no dependency on ServiceRegistry.
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                HandleBackButton();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) TryAutoPause();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) TryAutoPause();
        }

        // Services are resolved on demand rather than cached in Awake: this component is
        // added by GameBootstrap before ServiceRegistry.InstallDefaults() runs, and
        // ISceneFlowService is only registered later still, once a scene's
        // SceneFlowServiceInstaller has loaded. Both may legitimately be unavailable for a
        // few frames at startup — a background/back-button event that early is a no-op.

        private void TryAutoPause()
        {
            if (!ServiceRegistry.TryResolve<IGameStateMachine>(out var fsm)) return;
            if (fsm.Current is not GameplayState) return;
            if (!ServiceRegistry.TryResolve<ISceneFlowService>(out var sceneFlow)) return;

            AsyncUtils.FireAndForget(sceneFlow.GoToPauseAsync(), context: nameof(AppLifecycleHandler));
        }

        private void HandleBackButton()
        {
            if (!ServiceRegistry.TryResolve<IGameStateMachine>(out var fsm)) return;
            if (!ServiceRegistry.TryResolve<ISceneFlowService>(out var sceneFlow)) return;

            switch (fsm.Current)
            {
                case GameplayState:
                    AsyncUtils.FireAndForget(sceneFlow.GoToPauseAsync(), context: nameof(AppLifecycleHandler));
                    break;

                case PauseState:
                    AsyncUtils.FireAndForget(sceneFlow.ResumeFromPauseAsync(), context: nameof(AppLifecycleHandler));
                    break;

                case MainMenuState:
                    Application.Quit();
                    break;

                // ResultState and anything else: no default action, avoid surprising transitions.
            }
        }
    }
}
