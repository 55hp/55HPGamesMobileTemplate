using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

public class ToastQuickTest : MonoBehaviour
{
    private async void Start()
    {
        IUIToastService asd = null;
        for (int i = 0; i < 300 && asd == null; i++) // ~5s @60fps
        {
            if (ServiceRegistry.TryResolve(out asd))
                break;
            await Task.Yield();
        }
        if (asd == null)
        {
            Debug.LogError("IUIToastService non disponibile (91_UI_Root non ancora caricata?).");
            return;
        }

        // 2) Attendi il Canvas creato da 91_UI_Root
        Canvas canvas = null;
        for (int i = 0; i < 300 && canvas == null; i++)
        {
            canvas = FindFirstObjectByType<Canvas>();
            await Task.Yield();
        }
        if (canvas == null)
        {
            Debug.LogError("Canvas non trovato in scena.");
            return;
        }
        
        await Task.Yield();
        var toast = ServiceRegistry.Resolve<IUIToastService>();
        await toast.ShowAsync("Hello from Toast!", 1.8f);
        await toast.ShowAsync("Secondo toast in coda", 1.5f);
    }
}