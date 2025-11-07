using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

public class OptionsQuickTest : MonoBehaviour
{
    void Start()
    {
        var opt = ServiceRegistry.Resolve<IUIOptionsService>();
        opt.Load();
        opt.Changed += () => Debug.Log($"[Options] music={opt.MusicVolume:0.00} sfx={opt.SfxVolume:0.00} hapt={opt.Haptics} lang={opt.Language}");

        opt.MusicVolume = 0.5f;
        opt.SfxVolume   = 0.3f;
        opt.Haptics     = false;
        opt.Language    = "it";
        opt.Save();
    }
}