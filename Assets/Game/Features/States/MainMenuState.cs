using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Architecture.States
{
    /// <summary>
    /// Entry state for the template: shows the generic main menu page
    /// and starts the menu BGM.
    /// </summary>
    public sealed class MainMenuState : IGameState
    {
        private readonly IUINavigationService _nav;
        private readonly IMusicService _music;

        public MainMenuState()
        {
            if (!ServiceRegistry.TryResolve<IUINavigationService>(out _nav))
            {
                Debug.LogError("[MainMenuState] IUINavigationService not registered. " +
                               "Check that 91_UI_Root + UIServiceInstaller are loaded before starting the FSM.");
            }

            ServiceRegistry.TryResolve<IMusicService>(out _music);
        }

        public async Task EnterAsync(CancellationToken ct)
        {
            Debug.Log("[MainMenuState] Enter");

            // 1) Mostra la pagina di main menu (Addressable prefab)
            if (_nav != null)
            {
                await _nav.ReplaceAsync(global::hp55games.Addr.Content.UI.Pages.Main_Menu_Page);
            }

            // 2) Musica di menu, se il servizio è disponibile
            if (_music != null)
            {
                // Se il path non è questo, cambia solo la costante qui
                await _music.CrossfadeToAsync(
                    global::hp55games.Addr.Content.Audio.Bgm.MenuTheme,
                    0.5f
                );
            }
        }

        public Task ExitAsync(CancellationToken ct)
        {
            // Se vuoi, qui potresti fare pop della page o fermare la musica
            return Task.CompletedTask;
        }
    }
}