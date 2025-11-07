using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

public class OptionsPageQuickTest : MonoBehaviour
{
    private async void Start()
    {
        IUINavigationService asd = null;
        for (int i = 0; i < 300 && asd == null; i++) // ~5s @60fps
        {
            if (ServiceRegistry.TryResolve(out asd))
                break;
            await Task.Yield();
        }
        if (asd == null)
        {
            Debug.LogError("IUINavigationService non disponibile (91_UI_Root non ancora caricata?).");
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
        
        await Task.Yield();
        var nav = ServiceRegistry.Resolve<IUINavigationService>();
        await nav.PushAsync(hp55games.Addr.Content.UI.Pages.Options_Page);
    }
}