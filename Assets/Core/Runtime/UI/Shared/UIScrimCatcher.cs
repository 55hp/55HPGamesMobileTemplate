using UnityEngine;
using UnityEngine.EventSystems;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Aggancia questo al GameObject "Scrim" (con Image raycastTarget ON).
    /// Un click sullo scrim chiude il popup in cima allo stack.
    /// </summary>
    public sealed class UIScrimCatcher : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            if (ServiceRegistry.TryResolve<IUIPopupService>(out var pop))
                pop.CloseTop();
        }
    }
}