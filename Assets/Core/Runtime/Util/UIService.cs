// Assets/Core/Runtime/UI/UIService.cs
using UnityEngine;

namespace hp55games.Mobile.Core.Architecture
{
    public interface IUIService
    {
        void ToggleDevMenu();
        void Toast(string message);
    }

    public sealed class UIService : IUIService
    {
        // Implementazione minima/stub: collega più avanti ai tuoi prefab UI
        public void ToggleDevMenu()
        {
            Debug.Log("[UIService] ToggleDevMenu()");
        }

        public void Toast(string message)
        {
            Debug.Log($"[UIService] Toast: {message}");
        }
    }
}