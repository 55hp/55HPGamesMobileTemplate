using UnityEngine;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Static definition of an inventory item. IInventoryService tracks quantities keyed by
    /// the ItemData asset itself (identity-based), not by id string, so two different assets
    /// can never collide even if a designer types the same id twice — id is Inspector-facing
    /// metadata (e.g. for save data / analytics), not the runtime key.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItemData", menuName = "55HP/Progression/Item Data")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon = null;
        [SerializeField] private int maxStackSize = 1;

        [Tooltip("Plain string, not an enum — item categories are too project-specific to " +
                 "fix at template level.")]
        [SerializeField] private string category = string.Empty;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int MaxStackSize => maxStackSize;
        public string Category => category;
    }
}
