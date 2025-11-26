using System.Threading;
using System.Threading.Tasks;
using hp55games.Mobile.Core.Context;
using UnityEngine;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Architecture.States
{
    /// <summary>
    /// Gameplay state: reacts to gameplay scene already loaded by SceneFlowService.
    /// This state should only manage game logic, HUD, subscriptions, BGM, etc.
    /// </summary>
    public sealed class GameplayState : IGameState
    {
        private IMusicService _music;

        public async Task EnterAsync(CancellationToken ct)
        {
            Debug.Log("[GameplayState] Enter");

            // Optional: BGM setup
            if (ServiceRegistry.TryResolve<IMusicService>(out _music))
            {
                await _music.CrossfadeToAsync(Addr.Content.Audio.Bgm.GameTheme, 0.5f);
            }

            // 1) Inizializza il contesto di gioco (score e vite)
            IGameContextService context = null;
            ServiceRegistry.TryResolve(out context);

            if (context != null)
            {
                // Valori di default generici per il template
                context.Score = 0;
                context.Lives = 3; // Usa -1 se il gioco non usa vite
            }

            // 2) Mostra l'HUD di gameplay tramite il Navigation Service
            var navigation = ServiceRegistry.Resolve<IUINavigationService>();

            await navigation.ReplaceAsync(hp55games.Addr.Content.UI.Screens.GameplayHUD);
            
            await Task.Yield();
        }

        public async Task ExitAsync(CancellationToken ct)
        {
            Debug.Log("[GameplayState] Exit");

            // Optional cleanup (HUD, listeners, etc.)
            await Task.Yield();
        }
    }
}