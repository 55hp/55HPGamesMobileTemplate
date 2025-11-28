using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;
using hp55games.Mobile.Core.Context;

namespace hp55games.Mobile.Core.Architecture.States
{
    /// <summary>
    /// Results state:
    /// - Reacts to 03_Results scene already loaded by SceneFlowService.
    /// - Shows the generic results page.
    /// - Optionally sets a results BGM (for now reuse menu theme).
    /// </summary>
    public sealed class ResultState : IGameState
    {
        private readonly IUINavigationService _nav;
        private readonly IMusicService _music;
        private readonly IGameContextService _context;

        public ResultState()
        {
            if (!ServiceRegistry.TryResolve<IUINavigationService>(out _nav))
            {
                Debug.LogError("[ResultState] IUINavigationService not registered. " +
                               "Check that 91_UI_Root + UIServiceInstaller are loaded.");
            }

            // Music is optional
            ServiceRegistry.TryResolve<IMusicService>(out _music);

            // Context is optional, but very useful for score/lives
            ServiceRegistry.TryResolve<IGameContextService>(out _context);
        }

        public async Task EnterAsync(CancellationToken ct)
        {
            Debug.Log("[ResultState] Enter");

            if (_context != null)
            {
                Debug.Log($"[ResultState] Final score: {_context.Score}, lives: {_context.Lives}");
            }

            // 1) Mostra la pagina di risultati
            if (_nav != null)
            {
                // Usa la costante in Addr (la aggiungerai tu):
                // public const string Results_Page = "content/ui/pages/results";
                await _nav.ReplaceAsync(hp55games.Addr.Content.UI.Pages.Results_Page);
            }

            // 2) BGM dei risultati (per ora riutilizziamo il menu theme)
            if (_music != null)
            {
                await _music.CrossfadeToAsync(
                    global::hp55games.Addr.Content.Audio.Bgm.MenuTheme,
                    0.5f
                );
            }
        }

        public Task ExitAsync(CancellationToken ct)
        {
            Debug.Log("[ResultState] Exit");
            // Per ora non facciamo cleanup UI qui:
            // MainMenuState / GameplayState gestiranno la propria UI in Enter.
            return Task.CompletedTask;
        }
    }
}
