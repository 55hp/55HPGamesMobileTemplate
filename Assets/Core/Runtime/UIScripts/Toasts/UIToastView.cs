using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace hp55games.Mobile.UI
{
    public class UIToastView : MonoBehaviour
    {
        public TMP_Text label;
        public float lifetime = 2f;

        public async Task ShowAsync(string text)
        {
            if (label) label.text = text;

            var cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false; // non blocca input
            cg.alpha = 0f;

            // fade in
            float t = 0f;
            while (t < 0.12f)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.Clamp01(t / 0.12f);
                await Task.Yield();
            }
            cg.alpha = 1f;

            // hold
            float hold = lifetime;
            while (hold > 0f)
            {
                hold -= Time.unscaledDeltaTime;
                await Task.Yield();
            }

            // fade out
            t = 0f;
            while (t < 0.12f)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = 1f - Mathf.Clamp01(t / 0.12f);
                await Task.Yield();
            }
            cg.alpha = 0f;
        }
    }
}