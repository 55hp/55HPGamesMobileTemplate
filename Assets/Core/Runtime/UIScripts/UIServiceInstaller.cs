using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Registers all UI-level services once UIRoot scene (91_UI_Root) is loaded.
    /// This runs before we start the first GameState.
    /// </summary>
    public sealed class UIServiceInstaller : MonoBehaviour
    {
        private void Awake()
        {
            // UI services implementations (già esistenti nel tuo progetto)
            ServiceRegistry.Register<IUIPopupService>(new UIPopupService());
            ServiceRegistry.Register<IUINavigationService>(new UINavigationService());
            ServiceRegistry.Register<IUIOverlayService>(new UIOverlayService());
            ServiceRegistry.Register<IUIToastService>(new UIToastService());

            // Audio / music service (già creato come UIMusicService)
            ServiceRegistry.Register<IMusicService>(new UIMusicService());

            // Se hai IUIOptionsService e UIOptionsService, registra anche questo qui:
            ServiceRegistry.Register<IUIOptionsService>(new UIOptionsService());

            UIRuntime.MarkServicesReady();
        }
    }
}