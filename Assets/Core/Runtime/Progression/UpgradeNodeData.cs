using UnityEngine;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// One node in an upgrade tree/web. Prerequisites are asset references rather than id
    /// strings so broken links surface as missing-reference warnings in the Inspector instead
    /// of silently failing a string lookup at runtime.
    /// </summary>
    [CreateAssetMenu(fileName = "NewUpgradeNodeData", menuName = "55HP/Progression/Upgrade Node")]
    public class UpgradeNodeData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon = null;
        [SerializeField] private int cost = 0;
        [SerializeField] private CurrencyType costCurrency = CurrencyType.Soft;

        [Tooltip("Nodes that must already be unlocked before this one can be. " +
                 "Empty means this node has no prerequisites.")]
        [SerializeField] private UpgradeNodeData[] prerequisites = null;

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int Cost => cost;
        public CurrencyType CostCurrency => costCurrency;
        public UpgradeNodeData[] Prerequisites => prerequisites;
    }
}
