using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Architecture.States;

public class InitialStateInstaller : MonoBehaviour
{
    /// <summary>
    /// Avvia lo stato iniziale (MainMenuState) una volta che la scena 01_Menu è attiva.
    /// </summary>
    private async void Start()
    {
        // Risolvi la FSM dai servizi core
        var fsm = ServiceRegistry.Resolve<IGameStateMachine>();

        // Passa allo stato di Main Menu
        await fsm.ChangeStateAsync(new MainMenuState());
    }
}