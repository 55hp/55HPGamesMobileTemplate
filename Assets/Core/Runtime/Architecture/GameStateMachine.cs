using System.Threading.Tasks;
using hp55games.Mobile.Core.Runtime.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace hp55games.Mobile.Core.Architecture.States {
    public interface IGameState { Task Enter(); Task Exit(); }

    public sealed class GameStateMachine {
        public static GameStateMachine Instance { get; } = new();
        IGameState _current;
        public async void SetState(IGameState next){
            if (_current != null) await _current.Exit();
            _current = next;
            if (_current != null) await _current.Enter();
            
            Debug.Log("New state is: " + next.GetType().Name);
        }
    }
}


// Example state
namespace hp55games.Mobile.Core.Architecture.States {
    public sealed class MainMenuState : IGameState {
        public async Task Enter() {
            await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("01_Menu",LoadSceneMode.Additive);
        }
        public Task Exit() => System.Threading.Tasks.Task.CompletedTask;
    }
}