using System;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Gameplay.Events;
using hp55games.Mobile.Core.SceneFlow;

namespace hp55games.FlappyTsunami.Features.Gameplay
{
    /// <summary>
    /// Controller generico del flusso di gameplay per Flappy Tsunami.
    /// - Ascolta gli eventi di gameplay (es. PlayerDeathEvent).
    /// - Decide cosa fare a livello di flow (passare ai risultati, gestire achievements, ecc.).
    /// </summary>
    public class GameplayController : MonoBehaviour
    {
        [Header("Flow settings")]
        [SerializeField]
        private bool goToResultsOnPlayerDeath = true;

        private ISceneFlowService _sceneFlow;
        private IEventBus _eventBus;
        private IDisposable _deathSub;

        private void Awake()
        {
            _sceneFlow = ServiceRegistry.Resolve<ISceneFlowService>();
            _eventBus  = ServiceRegistry.Resolve<IEventBus>();

            // Pattern identico a qualsiasi altro listener di EventBus:
            // Subscribe in Awake / OnEnable, Unsubscribe in OnDestroy / OnDisable.
            _deathSub = _eventBus.Subscribe<PlayerDeathEvent>(OnPlayerDeath);
        }

        private void OnDestroy()
        {
                _deathSub?.Dispose();
        }

        private async void OnPlayerDeath(PlayerDeathEvent evt)
        {
            if (!goToResultsOnPlayerDeath)
                return;

            if (_sceneFlow == null)
            {
                Debug.LogWarning("[GameplayFlowController] SceneFlowService not available.");
                return;
            }

            Debug.Log("[GameplayFlowController] PlayerDeathEvent received -> GoToResultsAsync");
            await _sceneFlow.GoToResultsAsync();
        }

        // 🔜 In futuro puoi aggiungere altri handler qui:
        // - OnAchievementUnlocked(AchievementUnlockedEvent evt)
        // - OnRunStarted(RunStartedEvent evt)
        // - ecc.
    }
}
