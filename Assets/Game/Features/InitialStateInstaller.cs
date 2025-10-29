using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture.States;

public sealed class InitialStateInstaller : MonoBehaviour
{
    [SerializeField] private bool setOnAwake = false;

    private void Awake()
    {
        if (setOnAwake)
            SetInitialStateAsync().Forget();
    }

    private async void Start()
    {
        if (!setOnAwake)
            await SetInitialStateAsync();
    }

    private async Task SetInitialStateAsync()
    {
        // Qui puoi scegliere lo stato iniziale del tuo gioco (Game layer)
        await GameStateMachine.Instance.ChangeStateAsync(new MainMenuState());
    }
}

// Helper locale per “fire-and-forget” con log minimale (evita dipendenze dal Core)
static class TaskExt
{
    public static async void Forget(this Task task)
    {
        try { await task; } catch (System.Exception e) { Debug.LogException(e); }
    }
}