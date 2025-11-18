using System.Threading;
using System.Threading.Tasks;
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

            // TODO: load gameplay systems, HUD, etc.
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