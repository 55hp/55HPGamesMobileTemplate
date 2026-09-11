using UnityEngine;

namespace hp55games.Mobile.Core.UI
{
    /// <summary>
    /// Contract for showing contextual tooltips (drag hints, first-use explainers, ...).
    /// Pure interface, no base class — tooltip presentation (anchoring, fade, auto-dismiss
    /// timing) varies too much per project to share a MonoBehaviour skeleton, same reasoning
    /// as the Progression/Narrative service-only interfaces from Phases 4–5.
    ///
    /// HasBeenShown is keyed by an arbitrary string id the caller defines (e.g.
    /// "inventory_drag_hint"), not by the tooltip text itself — text can change
    /// (localization, copy iteration) without losing the "already seen" state tied to that id.
    /// </summary>
    public interface ITooltipService
    {
        void Show(string text, Vector2 screenPosition);

        void Hide();

        bool HasBeenShown(string tooltipId);
    }
}
