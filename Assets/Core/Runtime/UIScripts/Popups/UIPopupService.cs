using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;
using hp55games.Ui; // UIRoot

namespace hp55games.Mobile.UI
{
    public sealed class UIPopupService : IUIPopupService
    {
        private readonly IContentLoader _loader;
        private readonly List<GameObject> _opened = new();
        private UIRoot _ui;

        const float SCRIM_FADE = 0.12f;
        const float SCRIM_TARGET_ALPHA = 0.5f;

        public UIPopupService()
        {
            _loader = ServiceRegistry.Resolve<IContentLoader>();
            _ui = UIRoot.FindOrCache();
        }

        public async Task<GameObject> OpenAsync(string address)
        {
            await EnsureUIRootAsync();
            if (_ui == null) return null;

            // istanzia sotto Modals
            var popup = await _loader.InstantiateAsync(address, _ui.modals);
            if (popup == null) return null;

            _opened.Add(popup);

            // mostra scrim se è il primo
            if (_opened.Count == 1)
                await ScrimFadeToAsync(SCRIM_TARGET_ALPHA, true);

            return popup;
        }

        public async Task<T> OpenAsync<T>(string address) where T : Component
        {
            var go = await OpenAsync(address);
            return go ? go.GetComponent<T>() : null;
        }

        public void Close(GameObject popup)
        {
            if (popup == null) return;

            if (_opened.Remove(popup))
            {
                _loader.ReleaseInstance(popup);
                if (_opened.Count == 0)
                    _ = ScrimFadeToAsync(0f, false); // nascondi scrim
            }
            else
            {
                _loader.ReleaseInstance(popup);
            }
        }

        public void CloseTop()
        {
            if (_opened.Count == 0) return;
            var top = _opened[_opened.Count - 1];
            Close(top);
        }

        public void CloseAll()
        {
            foreach (var p in _opened)
                if (p != null) _loader.ReleaseInstance(p);
            _opened.Clear();
            _ = ScrimFadeToAsync(0f, false);
        }

        // -------- helpers --------

        private async Task EnsureUIRootAsync()
        {
            if (_ui != null) return;
            for (int i = 0; i < 180 && _ui == null; i++)
            {
                _ui = UIRoot.FindOrCache();
                await Task.Yield();
            }
            if (_ui == null)
                Debug.LogError("[UIPopupService] UIRoot non trovato.");
        }

        private async Task ScrimFadeToAsync(float targetAlpha, bool enableRaycast)
        {
            if (_ui == null || _ui.scrim == null) return;

            var cg = _ui.scrim.GetComponent<CanvasGroup>();
            if (cg == null) cg = _ui.scrim.gameObject.AddComponent<CanvasGroup>();

            cg.blocksRaycasts = enableRaycast;
            cg.interactable   = false;

            float start = cg.alpha;
            float t = 0f;
            while (t < SCRIM_FADE)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(start, targetAlpha, t / SCRIM_FADE);
                await Task.Yield();
            }
            cg.alpha = targetAlpha;

            if (Mathf.Approximately(targetAlpha, 0f))
                cg.blocksRaycasts = false;
        }
    }
}
