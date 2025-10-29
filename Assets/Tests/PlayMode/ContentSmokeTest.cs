using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;

public class ContentSmokeTest : MonoBehaviour
{
    private async void Start()
    {
        // 1) Attendi il Canvas (come già fatto)
        Canvas canvas = null;
        for (int i = 0; i < 180 && canvas == null; i++)
        {
            canvas = FindFirstObjectByType<Canvas>();
            await Task.Yield();
        }
        if (canvas == null) { Debug.LogError("Canvas non trovato"); return; }

        // 2) Instanzia direttamente via Addressables (niente Instantiate+SetParent manuale)
        var loader = ServiceRegistry.Resolve<IContentLoader>();
        var popup = await loader.InstantiateAsync(hp55games.Addr.Content.UI.Popup_Generic, canvas.transform);

        Debug.Log("[ContentSmokeTest] Popup instanziato via Addressables.InstantiateAsync");

        // (Facoltativo) dopo X secondi rilascia
        // await Task.Delay(2000);
        // loader.ReleaseInstance(popup);
    }
}