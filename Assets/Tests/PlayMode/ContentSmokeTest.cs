using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;

public class ContentSmokeTest : MonoBehaviour
{
    private async void Start()
    {
        var loader = ServiceRegistry.Resolve<IContentLoader>();

        // 1) Carica il prefab addressable
        var prefab = await loader.LoadAsync<GameObject>(hp55games.Addr.Content.UI.Popup_Generic);
        if (prefab == null) { Debug.LogError("Popup prefab null"); return; }

        // 2) Trova un Canvas già in scena (da 91_UI_Root)
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Nessun Canvas trovato in scena. Carica 91_UI_Root prima di istanziare UI.");
            return;
        }

        // 3) Instanzia e fai il parenting sotto il Canvas (mantieni ancoraggi)
        var go = Instantiate(prefab);
        var t = go.transform as RectTransform;
        t.SetParent(canvas.transform, false); // false = conserva ancoraggi/scala del prefab

        // (opz) centra l’ancoraggio se necessario
        t.anchoredPosition = Vector2.zero;

        Debug.Log("[ContentSmokeTest] Popup instanziato sotto Canvas.");
    }
}