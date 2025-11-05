using UnityEngine;

namespace hp55games.Ui
{
    /// <summary>
    /// UIRoot vive sotto il Canvas (unico). I 7 layer sono figli del Canvas.
    /// L'ordine dei sibling determina lo z-order:
    /// HUD(0) → Pages(1) → Drawers(2) → Scrim(3) → Modals(4) → Toasts(5) → Overlays(6)
    /// </summary>
    public class UIRoot : MonoBehaviour
    {
        [Header("Riferimenti (figli del Canvas)")]
        public RectTransform hud;       // 0
        public RectTransform pages;     // 1
        public RectTransform drawers;   // 2
        public RectTransform scrim;     // 3 (Image nero semi, raycastTarget ON)
        public RectTransform modals;    // 4
        public RectTransform toasts;    // 5 (non blocca input)
        public RectTransform overlays;  // 6 (fade/loading/blocker)

        private static UIRoot _cached;
        public static UIRoot FindOrCache()
        {
            if (_cached == null) _cached = FindFirstObjectByType<UIRoot>();
            return _cached;
        }

        private void OnValidate()
        {
            // Mantieni l'ordine desiderato
            TrySetSibling(hud, 0);
            TrySetSibling(pages, 1);
            TrySetSibling(drawers, 2);
            TrySetSibling(scrim, 3);
            TrySetSibling(modals, 4);
            TrySetSibling(toasts, 5);
            TrySetSibling(overlays, 6);
        }

        private static void TrySetSibling(RectTransform t, int index)
        {
            if (t != null && t.GetSiblingIndex() != index)
                t.SetSiblingIndex(index);
        }
    }
}