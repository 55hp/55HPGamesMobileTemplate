using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Architecture.States
{
    /// <summary>
    /// Simple gameplay state: loads 02_Gameplay scene additively,
    /// sets up music, and will later handle gameplay-specific logic.
    /// </summary>
    public sealed class ResultState : IGameState
    {
        public Task EnterAsync(CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }

        public Task ExitAsync(CancellationToken ct)
        {
            throw new System.NotImplementedException();
        }
    }
}