using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Context;
using hp55games.Mobile.Core.SceneFlow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace hp55games.Mobile.Game.UI
{
    /// <summary>
    /// Generic gameplay HUD:
    /// - Displays current score and lives (if available).
    /// - Exposes methods to update score/lives from gameplay code.
    /// - Pause button uses SceneFlowService to enter PauseState.
    /// </summary>
    public sealed class UIGameplayHUD : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _scoreLabel;
        [SerializeField] private TextMeshProUGUI _livesLabel;
        [SerializeField] private Button _pauseButton;

        private IGameContextService _context;
        private ISceneFlowService _sceneFlow;

        private void Awake()
        {
            // Resolve services (if available)
            ServiceRegistry.TryResolve(out _context);
            _sceneFlow = ServiceRegistry.Resolve<ISceneFlowService>();

            // Init UI from context (if present)
            int score = 0;
            int lives = -1;

            if (_context != null)
            {
                score = _context.Score;
                lives = _context.Lives;
            }

            UpdateScoreLabel(score);
            UpdateLivesLabel(lives);

            if (_pauseButton != null)
                _pauseButton.onClick.AddListener(OnPauseClicked);
        }

        private void OnDestroy()
        {
            if (_pauseButton != null)
                _pauseButton.onClick.RemoveListener(OnPauseClicked);
        }

        // --- Public API for gameplay code ---

        public void SetScore(int newScore)
        {
            UpdateScoreLabel(newScore);

            if (_context != null)
                _context.Score = newScore;
        }

        public void SetLives(int newLives)
        {
            UpdateLivesLabel(newLives);

            if (_context != null)
                _context.Lives = newLives;
        }

        // --- Internal helpers ---

        private void UpdateScoreLabel(int value)
        {
            if (_scoreLabel != null)
                _scoreLabel.text = value.ToString();
        }

        private void UpdateLivesLabel(int value)
        {
            if (_livesLabel == null)
                return;

            if (value < 0)
            {
                // -1 or less = "no lives system", hide the label
                _livesLabel.gameObject.SetActive(false);
            }
            else
            {
                _livesLabel.gameObject.SetActive(true);
                _livesLabel.text = value.ToString();
            }
        }

        private async void OnPauseClicked()
        {
            if (_sceneFlow == null)
            {
                Debug.LogWarning("[UIGameplayHUD] ISceneFlowService not found. Pause button will do nothing.");
                return;
            }

            // Assumes ISceneFlowService has a GoToPauseAsync() method
            await _sceneFlow.GoToPauseAsync();
        }
    }
}
