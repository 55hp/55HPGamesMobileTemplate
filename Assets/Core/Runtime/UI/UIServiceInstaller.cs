using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    public sealed class UIServiceInstaller : MonoBehaviour
    {
        void Awake()
        {
            ServiceRegistry.Register<IUIPopupService>(new UIPopupService());
            //TODO implementa gli altri servizi
        }
    }
}