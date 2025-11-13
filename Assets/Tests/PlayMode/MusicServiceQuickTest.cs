using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

public class MusicServiceQuickTest : MonoBehaviour
{
    private async void Start()
    {
        IMusicService asd = null;
        for (int i = 0; i < 300 && asd == null; i++) // ~5s @60fps
        {
            if (ServiceRegistry.TryResolve(out asd))
                break;
            await Task.Yield();
        }
        if (asd == null)
        {
            Debug.LogError("IMusicService non disponibile (91_UI_Root non ancora caricata?).");
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
        
        var music = ServiceRegistry.Resolve<IMusicService>();

        await music.PlayAsync("content/audio/bgm/menu_theme", 0.3f);
        await Task.Delay(10000);

        await music.CrossfadeToAsync("content/audio/bgm/game_theme", 1.0f);
        await Task.Delay(2000);

        await music.StopAsync(0.5f);
    }
}