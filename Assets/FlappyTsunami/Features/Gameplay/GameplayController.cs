using System;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Context;
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
        private ISceneFlowService _sceneFlow;
        private IEventBus _eventBus;
        private IDisposable _deathSub;
        private IGameContextService _context;
        private ISaveService _save;

        private void Awake()
        {
            _sceneFlow = ServiceRegistry.Resolve<ISceneFlowService>();
            _eventBus  = ServiceRegistry.Resolve<IEventBus>();
            _save= ServiceRegistry.Resolve<ISaveService>();
            _context = ServiceRegistry.Resolve<IGameContextService>();
            _deathSub = _eventBus.Subscribe<PlayerDeathEvent>(OnPlayerDeath);
        }

        private void OnDestroy()
        {
                _deathSub?.Dispose();
        }

        private async void OnPlayerDeath(PlayerDeathEvent _)
        {
            if (_context == null || _save == null)
            {
                Debug.LogWarning("[GameplayController] Context o Save mancano quando arriva PlayerDeathEvent.");
                return;
            }

            var currentScore = _context.Score;

            // Leggi best esistente
            _save.Load();
            var bestScore = _save.Data.progress.bestScore;

            // Se questa run è meglio, aggiorna e salva
            if (currentScore > bestScore)
            {
                _save.Data.progress.bestScore = currentScore;
                _save.Save();

                // TODO: qui in futuro puoi triggerare un feedback "NUOVO RECORD!"
            }

            // Aggiorna il contesto così UI / ResultState leggono il valore giusto
            _context.BestScore = bestScore;

            // Vai ai risultati
            if (_sceneFlow != null)
            {
                await _sceneFlow.GoToResultsAsync();
            }
        }
        // 🔜 In futuro puoi aggiungere altri handler qui:
        // - OnAchievementUnlocked(AchievementUnlockedEvent evt)
        // - OnRunStarted(RunStartedEvent evt)
        // - ecc.
    }
}
