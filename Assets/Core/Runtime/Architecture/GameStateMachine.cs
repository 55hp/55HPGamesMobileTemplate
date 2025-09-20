using System.Threading.Tasks;
using AF55HP.Mobile.Core.Runtime.Util;

namespace AF55HP.Mobile.Core.Architecture.States {
    public interface IGameState { Task Enter(); Task Exit(); }

    public sealed class GameStateMachine {
        public static GameStateMachine Instance { get; } = new();
        IGameState _current;
        public async void SetState(IGameState next){
            if (_current != null) await _current.Exit();
            _current = next;
            if (_current != null) await _current.Enter();
        }
    }
}


// Example state
namespace AF55HP.Mobile.Core.Architecture.States {
    public sealed class MainMenuState : IGameState {
        public async Task Enter() {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("01_Menu");
        }
        public Task Exit() => System.Threading.Tasks.Task.CompletedTask;
    }
}