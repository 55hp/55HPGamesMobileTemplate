using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.Architecture.States;
using hp55games.Mobile.Core.Time;

public class InitialStateInstaller : MonoBehaviour
{
    /// <summary>
    /// Avvia lo stato iniziale (MainMenuState) una volta che la scena 01_Menu è attiva.
    /// Qui calcoliamo anche da quanto tempo manca l'ultima sessione.
    /// </summary>
    private async void Start()
    {
        var fsm  = ServiceRegistry.Resolve<IGameStateMachine>();
        var time = ServiceRegistry.Resolve<ITimeService>();

        // 1) Quanto tempo è passato dall'ultima sessione?
        var since       = time.SinceLast("last_session");
        var minutesAway = since.TotalMinutes;

        Debug.Log($"[Session] Last session: {minutesAway:F1} minutes ago.");

        // 2) Entra nello stato di Main Menu
        await fsm.ChangeStateAsync(new MainMenuState());

        // 3) Solo dopo aver avviato il menu aggiorniamo "last_session"
        time.SetNowUtc("last_session");
    }
}