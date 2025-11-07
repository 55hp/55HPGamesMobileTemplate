using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Aggancialo in 91_UI_Root (qualsiasi GO). Registra i servizi UI.
    /// </summary>
    public sealed class UIServiceInstaller : MonoBehaviour
    {
        private void Awake()
        {
            ServiceRegistry.Register<IUIPopupService>(new UIPopupService());
            ServiceRegistry.Register<IUINavigationService>(new UINavigationService());
            ServiceRegistry.Register<IUIOverlayService>(new UIOverlayService());
            ServiceRegistry.Register<IUIToastService>(new UIToastService());
        }
    }
}