using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;
using hp55games.Ui; // UIRoot

namespace hp55games.Mobile.UI
{
    /// <summary>Mostra toast non bloccanti in coda, parentati su UIRoot.toasts.</summary>
    public sealed class UIToastService : IUIToastService
    {
        private readonly IContentLoader _loader;
        private readonly Queue<(string msg, float sec)> _queue = new();
        private bool _isShowing;
        private UIRoot _ui;

        private const string TOAST_ADDR = hp55games.Addr.Content.UI.Toasts.Default;

        public UIToastService()
        {
            _loader = ServiceRegistry.Resolve<IContentLoader>();
            _ui = UIRoot.FindOrCache();
        }

        public async Task ShowAsync(string message, float seconds = 2f)
        {
            _queue.Enqueue((message, seconds));
            if (!_isShowing)
                await PumpAsync();
        }

        private async Task PumpAsync()
        {
            _isShowing = true;
            await EnsureUIRootAsync();
            if (_ui == null) { _isShowing = false; return; }

            while (_queue.Count > 0)
            {
                var (msg, sec) = _queue.Dequeue();

                var go = await _loader.InstantiateAsync(TOAST_ADDR, _ui.toasts);
                if (go == null) continue;

                // non blocca input
                var cg = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();
                cg.blocksRaycasts = false;

                // set testo + durata
                var view = go.GetComponent<UIToastView>(); // opzionale ma consigliato
                if (view != null)
                {
                    view.lifetime = sec;
                    await view.ShowAsync(msg);
                }
                else
                {
                    // fallback: resta visibile per sec e poi sparisce
                    await Task.Delay(Mathf.RoundToInt(sec * 1000));
                }

                _loader.ReleaseInstance(go);
                // mini pausa tra toasts
                await Task.Delay(75);
            }

            _isShowing = false;
        }

        private async Task EnsureUIRootAsync()
        {
            if (_ui != null) return;
            for (int i = 0; i < 180 && _ui == null; i++)
            {
                _ui = UIRoot.FindOrCache();
                await Task.Yield();
            }
            if (_ui == null)
                Debug.LogError("[UIToastService] UIRoot non trovato (91_UI_Root?).");
        }
    }
}
