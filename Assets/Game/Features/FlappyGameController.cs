using System;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Context;
using hp55games.Mobile.Core.Gameplay.Events;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.Game.Events;
using hp55games.Mobile.Game.UI;

namespace hp55games.Mobile.Game.FlappyTest
{
    /// <summary>
    /// Minimal game controller for the Flappy test game.
    /// - Keeps track of score & lives for this run.
    /// - Updates the UIGameplayHUD.
    /// - On death, stores final score in GameContext and goes to Results.
    /// 
    /// This is NOT core; it's specific to the FlappyTest prototype.
    /// </summary>
    public sealed class FlappyGameController : MonoBehaviour
    {
        [Header("Run Settings")]
        [SerializeField] private int _startingLives = 1;

        private IGameContextService _context;
        private ISceneFlowService _sceneFlow;
        private IEventBus _bus;

        private int _score;
        private int _lives;

        private bool _isGameOver;

        private IDisposable _pipePassedSub;

        private void Awake()
        {
            ServiceRegistry.TryResolve(out _context);
            _sceneFlow = ServiceRegistry.Resolve<ISceneFlowService>();
            _bus = ServiceRegistry.Resolve<IEventBus>();
            
            _score = 0;
            _lives = _startingLives;
            
            if (_context != null)
            {
                _context.Score = _score;
                _context.Lives = _lives;
            }
            
            _bus?.Publish(new HpChangedEvent());
            _bus?.Publish(new ScoreChangedEvent());
            
            if (_bus != null)
            {
                _pipePassedSub = _bus.Subscribe<PipePassedEvent>(_ => OnPipePassed());
            }
        }

        private void OnDestroy()
        {
            _pipePassedSub?.Dispose();
        }
        
        private void OnPipePassed()
        {
            AddScore(1);
        }

        /// <summary>
        /// Call this whenever the player successfully passes an obstacle / gains points.
        /// </summary>
        public void AddScore(int amount)
        {
            if (_isGameOver)
                return;

            _score += amount;

            if (_context != null)
                _context.Score = _score;
            
            _bus?.Publish(new ScoreChangedEvent());
        }

        public void OnPlayerDeath()
        {
            if (_isGameOver)
                return;

            _isGameOver = true;

            _lives--;

            if (_context != null)
            {
                _context.Lives = _lives;
                _context.Score = _score;
            }
            
            _bus?.Publish(new PlayerDeathEvent());
            
            GoToResults();
        }

        private async void GoToResults()
        {
            if (_sceneFlow == null)
            {
                Debug.LogWarning("[FlappyGameController] ISceneFlowService not found. Cannot go to Results.");
                return;
            }

            await _sceneFlow.GoToResultsAsync();
        }
    }
}
