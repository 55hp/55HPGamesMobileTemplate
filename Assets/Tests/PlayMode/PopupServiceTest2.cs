using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;
using hp55games.Ui;

public class PopupServiceTest2 : MonoBehaviour
{
    private async void Start()
    {
        // 1) Attendi che l'installer UI abbia registrato il servizio
        IUIPopupService asd = null;
        for (int i = 0; i < 300 && asd == null; i++) // ~5s @60fps
        {
            if (ServiceRegistry.TryResolve(out asd))
                break;
            await Task.Yield();
        }
        if (asd == null)
        {
            Debug.LogError("UIPopupService non disponibile (91_UI_Root non ancora caricata?).");
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
        
        // attesa breve per dare tempo a 91_UI_Root di registrare i servizi
        await Task.Yield();

        var svc = ServiceRegistry.Resolve<IUIPopupService>();

        // 👇 ora ottieni direttamente il componente
        var popup = await svc.OpenAsync<UIPopup_Generic>(hp55games.Addr.Content.UI.Popup_Generic);
        if (popup != null)
        {
            // esempio: setta un testo o collega un bottone
            // popup.SetMessage("Hello!");
            await Task.Delay(1200);
            svc.Close(popup.gameObject);
        }
    }
}