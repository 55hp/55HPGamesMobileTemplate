using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;
using hp55games.Ui; // UIRoot

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Gestione stack di pagine (Addressables) con fade.
    /// Parent automatico: UIRoot/Pages.
    /// </summary>
    public sealed class UINavigationService : IUINavigationService
    {
        private readonly IContentLoader _loader;
        private readonly Stack<PageEntry> _stack = new();
        private RectTransform _pagesRoot;

        private const float FadeDuration = 0.15f;

        public UINavigationService()
        {
            _loader = ServiceRegistry.Resolve<IContentLoader>();
        }

        public bool CanGoBack => _stack.Count > 1;

        public async Task PushAsync(string address)
        {
            await EnsurePagesRootAsync();
            if (_pagesRoot == null) return;

            var go = await _loader.InstantiateAsync(address, _pagesRoot);
            if (go == null)
            {
                Debug.LogError($"[UINavigationService] Instanziazione pagina fallita: '{address}'");
                return;
            }

            var entry = new PageEntry(address, go);

            if (_stack.Count > 0)
            {
                var prev = _stack.Peek();
                await FadeOut(prev.GameObject);
                prev.GameObject.SetActive(false);
            }

            go.SetActive(true);
            await FadeIn(go);

            _stack.Push(entry);
        }

        public async Task ReplaceAsync(string address)
        {
            if (_stack.Count > 0)
                await PopAsync();

            await PushAsync(address);
        }

        public async Task PopAsync()
        {
            if (_stack.Count == 0) return;

            var current = _stack.Pop();
            await FadeOut(current.GameObject);
            _loader.ReleaseInstance(current.GameObject);

            if (_stack.Count > 0)
            {
                var prev = _stack.Peek();
                prev.GameObject.SetActive(true);
                await FadeIn(prev.GameObject);
            }
        }

        // ---------- helpers ----------

        private async Task EnsurePagesRootAsync()
        {
            if (_pagesRoot != null) return;

            UIRoot ui = UIRoot.FindOrCache();
            for (int i = 0; i < 180 && ui == null; i++)
            {
                await Task.Yield();
                ui = UIRoot.FindOrCache();
            }
            if (ui == null || ui.pages == null)
            {
                Debug.LogError("[UINavigationService] UIRoot o 'Pages' non trovato.");
                return;
            }
            _pagesRoot = ui.pages;
        }

        private static CanvasGroup RequireCanvasGroup(GameObject go)
        {
            var cg = go.GetComponent<CanvasGroup>();
            if (cg == null) cg = go.AddComponent<CanvasGroup>();
            return cg;
        }

        private static async Task FadeIn(GameObject go)
        {
            var cg = RequireCanvasGroup(go);
            cg.interactable = false;
            cg.blocksRaycasts = false;

            float t = 0f;
            while (t < FadeDuration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Clamp01(t / FadeDuration);
                await Task.Yield();
            }
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        private static async Task FadeOut(GameObject go)
        {
            var cg = RequireCanvasGroup(go);
            cg.interactable = false;
            cg.blocksRaycasts = false;

            float t = 0f;
            float start = cg.alpha;
            while (t < FadeDuration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Lerp(start, 0f, t / FadeDuration);
                await Task.Yield();
            }
            cg.alpha = 0f;
        }

        private readonly struct PageEntry
        {
            public readonly string Address;
            public readonly GameObject GameObject;
            public PageEntry(string address, GameObject go) { Address = address; GameObject = go; }
        }
    }
}
