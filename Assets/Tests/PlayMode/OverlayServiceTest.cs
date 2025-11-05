using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

public class OverlayServiceTest : MonoBehaviour
{
    private async void Start()
    {
        IUIOverlayService asd = null;
        for (int i = 0; i < 300 && asd == null; i++) // ~5s @60fps
        {
            if (ServiceRegistry.TryResolve(out asd))
                break;
            await Task.Yield();
        }
        if (asd == null)
        {
            Debug.LogError("IUIOverlayService non disponibile (91_UI_Root non ancora caricata?).");
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
        var overlay = ServiceRegistry.Resolve<IUIOverlayService>();

        await overlay.FadeInAsync(0.15f);
        await Task.Delay(400);
        overlay.BlockInput(true);

        await overlay.ShowLoadingAsync("Caricamento…");
        await Task.Delay(1000);
        overlay.HideLoading();

        await overlay.FadeOutAsync(0.15f);
        overlay.BlockInput(false);
    }
}