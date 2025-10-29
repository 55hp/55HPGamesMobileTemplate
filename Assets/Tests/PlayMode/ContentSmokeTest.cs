using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;

public class ContentSmokeTest : MonoBehaviour
{
    private async void Start()
    {
        // 1) Attendi che il Canvas sia disponibile (max ~3 sec)
        Canvas canvas = null;
        for (int i = 0; i < 180 && canvas == null; i++) // 180 frame ~ 3s @60fps
        {
            canvas = FindFirstObjectByType<Canvas>();
            await Task.Yield();
        }

        if (canvas == null)
        {
            Debug.LogError("Nessun Canvas trovato (anche dopo attesa). Verifica che 91_UI_Root sia in scena.");
            return;
        }

        // 2) Carica e istanzia il popup sotto il Canvas
        var loader = ServiceRegistry.Resolve<IContentLoader>();
        var prefab = await loader.LoadAsync<GameObject>(hp55games.Addr.Content.UI.Popup_Generic);
        if (prefab == null) { Debug.LogError("Popup prefab null"); return; }

        var go = Instantiate(prefab);
        var rt = go.transform as RectTransform;
        rt.SetParent(canvas.transform, false);
        rt.anchoredPosition = Vector2.zero;

        Debug.Log("[ContentSmokeTest] Popup instanziato sotto Canvas (dopo attesa).");
    }
}