using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Context;
using hp55games.Mobile.Core.SceneFlow;
using hp55games.Mobile.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace hp55games.Mobile.Game.UI
{
    /// <summary>
    /// Generic Results page:
    /// - Shows final score from GameContext.
    /// - Retry -> go back to Gameplay via SceneFlowService.
    /// - Main Menu -> go back to Menu via SceneFlowService.
    /// </summary>
    public sealed class UIResultsPage : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private UILocalizedText _scoreLabel;
        [SerializeField] private UILocalizedText _bestScoreLabel;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;

        private ISaveService _save;
        private IGameContextService _context;
        private ISceneFlowService _sceneFlow;

        private void Awake()
        {
            ServiceRegistry.TryResolve(out _save);
            ServiceRegistry.TryResolve(out _context);
            _sceneFlow = ServiceRegistry.Resolve<ISceneFlowService>();

            int score = _context?.Score ?? 0;

            if (_scoreLabel != null)
            {
                _scoreLabel.SetSuffix(" :" + score.ToString());
                _scoreLabel.Refresh();
            }

            if (_retryButton != null)
                _retryButton.onClick.AddListener(OnRetryClicked);

            if (_menuButton != null)
                _menuButton.onClick.AddListener(OnMenuClicked);
        }
        
        private void Start()
        {
            int finalScore = _context != null ? _context.Score : 0;
            int bestScore  = _save != null ? _save.Data.progress.bestScore : 0;

            if (_scoreLabel != null)
            {
                _scoreLabel.SetSuffix(" :" + finalScore);
                _scoreLabel.Refresh();
            }

            if (_bestScoreLabel != null)
            {
                _bestScoreLabel.SetSuffix(" :" + bestScore);
                _bestScoreLabel.Refresh();
            }
        }

        private void OnDestroy()
        {
            if (_retryButton != null)
                _retryButton.onClick.RemoveListener(OnRetryClicked);

            if (_menuButton != null)
                _menuButton.onClick.RemoveListener(OnMenuClicked);
        }

        private async void OnRetryClicked()
        {
            if (_sceneFlow == null)
            {
                Debug.LogWarning("[UIResultsPage] ISceneFlowService not found. Retry will do nothing.");
                return;
            }

            await _sceneFlow.GoToGameplayAsync();
        }

        private async void OnMenuClicked()
        {
            if (_sceneFlow == null)
            {
                Debug.LogWarning("[UIResultsPage] ISceneFlowService not found. Menu will do nothing.");
                return;
            }

            await _sceneFlow.GoToMenuAsync();
        }
    }
}
