using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Architecture.States
{
    /// <summary>
    /// Simple gameplay state: loads 02_Gameplay scene additively,
    /// sets up music, and will later handle gameplay-specific logic.
    /// </summary>
    public sealed class GameplayState : IGameState
    {
        private IMusicService _music;

        public async Task EnterAsync(CancellationToken ct)
        {
            Debug.Log("[GameplayState] Enter");

            // Load gameplay scene additively if not yet loaded
            var sceneName = "Scenes/02_Gameplay";
            if (!SceneManager.GetSceneByPath(sceneName).isLoaded)
            {
                var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!op.isDone && !ct.IsCancellationRequested)
                    await Task.Yield();
            }

            // music (optional, puoi cambiare la track key)
            if (ServiceRegistry.TryResolve<IMusicService>(out _music))
            {
                // Usa una BGM diversa dal menu, ad esempio:
                // Addr.Content.Audio.Bgm.GameplayTheme
                await _music.CrossfadeToAsync(Addr.Content.Audio.Bgm.GameTheme, 0.5f);
            }
        }

        public async Task ExitAsync(CancellationToken ct)
        {
            Debug.Log("[GameplayState] Exit");

            // opzionale: scaricare la scena gameplay quando esci
            var sceneName = "Scenes/02_Gameplay";
            var scene = SceneManager.GetSceneByPath(sceneName);
            if (scene.isLoaded)
            {
                var op = SceneManager.UnloadSceneAsync(scene);
                while (!op.isDone && !ct.IsCancellationRequested)
                    await Task.Yield();
            }

            await Task.CompletedTask;
        }
    }
}