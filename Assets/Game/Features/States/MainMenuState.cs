using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Architecture.States
{
    public sealed class MainMenuState : IGameState
    {
        private IMusicService _music;

        public MainMenuState()
        {
            // NON risolviamo servizi nel costruttore.
        }

        public async Task EnterAsync(CancellationToken ct)
        {
            Debug.Log("[MainMenuState] Enter");

            // Aspetta che IMusicService venga registrato (fino a ~2 secondi)
            if (_music == null)
            {
                const int maxFrames = 120; // ~2s a 60fps
                for (int i = 0; i < maxFrames && _music == null && !ct.IsCancellationRequested; i++)
                {
                    if (ServiceRegistry.TryResolve<IMusicService>(out _music))
                        break;

                    // aspetta il frame successivo senza bloccare
                    await Task.Yield();
                }

                if (_music == null)
                {
                    Debug.LogWarning("[MainMenuState] IMusicService non disponibile dopo il timeout; nessuna musica di menu.");
                    return;
                }
            }

            // Avvia o crossfada il BGM del menu
            await _music.CrossfadeToAsync(hp55games.Addr.Content.Audio.Bgm.MenuTheme, 0.5f);
        }

        public async Task ExitAsync(CancellationToken ct)
        {
            Debug.Log("[MainMenuState] Exit");

            // Se vuoi fermare la musica in uscita dal menu, puoi decommentare:
            // if (_music != null)
            //     await _music.StopAsync(0.3f);

            await Task.CompletedTask;
        }
    }
}