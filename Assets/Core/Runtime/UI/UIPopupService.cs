using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;

namespace hp55games.Mobile.Core.UI
{
    /// <summary>
    /// Gestione popup UI via Addressables.
    /// Usa IContentLoader del ServiceRegistry.
    /// </summary>
    public sealed class UIPopupService : IUIPopupService
    {
        private readonly IContentLoader _loader;
        private readonly List<GameObject> _opened = new();

        public UIPopupService()
        {
            _loader = ServiceRegistry.Resolve<IContentLoader>();
        }
        
        public async Task<T> OpenAsync<T>(string address) where T : Component
        {
            var go = await OpenAsync(address);
            if (go == null) return null;

            var comp = go.GetComponent<T>();
            if (comp == null)
                Debug.LogWarning($"[UIPopupService] Il componente {typeof(T).Name} non è presente su '{go.name}'.");

            return comp;
        }

        public async Task<GameObject> OpenAsync(string address)
        {
            // trova il Canvas principale
            var canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("[UIPopupService] Nessun Canvas trovato in scena.");
                return null;
            }

            // istanzia il prefab come figlio del Canvas
            var popup = await _loader.InstantiateAsync(address, canvas.transform);
            if (popup != null)
            {
                _opened.Add(popup);
            }

            return popup;
        }

        public void Close(GameObject popup)
        {
            if (popup == null) return;

            if (_opened.Contains(popup))
            {
                _opened.Remove(popup);
            }

            _loader.ReleaseInstance(popup);
        }

        public void CloseAll()
        {
            foreach (var popup in _opened)
            {
                if (popup != null)
                    _loader.ReleaseInstance(popup);
            }
            _opened.Clear();
        }
    }
}