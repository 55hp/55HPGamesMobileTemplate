using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    public sealed class UIServiceInstaller : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("[UIServiceInstaller] Awake → registro servizi UI");

            ServiceRegistry.Register<IUIPopupService>(new UIPopupService());
            ServiceRegistry.Register<IUINavigationService>(new UINavigationService());
            ServiceRegistry.Register<IUIOverlayService>(new UIOverlayService());
            ServiceRegistry.Register<IUIToastService>(new UIToastService());

            // 👇 DEVE ESSERCI
            ServiceRegistry.Register<IMusicService>(new UIMusicService());
        }
    }
}