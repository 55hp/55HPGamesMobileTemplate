using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture.States;

public sealed class MainMenuState : IGameState
{
    public async Task EnterAsync(CancellationToken ct)
    {
        // Esempio dummy async:
        await Task.Yield();
        Debug.Log("[MainMenuState] Enter");
    }

    public async Task ExitAsync(CancellationToken ct)
    {
        // Chiudi popup, salva, ecc.
        await Task.Yield();
        Debug.Log("[MainMenuState] Exit");
    }
}