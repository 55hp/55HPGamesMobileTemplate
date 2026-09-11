using System;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Contract for tracking item quantities. Pure interface, no base class: unlike the
    /// Combat/AI/Camera phases, progression & economy logic (stacking rules, persistence,
    /// server-authoritative balances, ...) varies too much per project to share a MonoBehaviour
    /// skeleton — a concrete project implements this directly and registers it via
    /// ServiceRegistry.Register&lt;IInventoryService&gt;(...), it is NOT installed by default.
    /// </summary>
    public interface IInventoryService
    {
        bool TryAddItem(ItemData item, int quantity);

        bool TryRemoveItem(ItemData item, int quantity);

        int GetItemCount(ItemData item);

        event Action<ItemData, int> OnItemCountChanged;
    }
}
