using UnityEngine;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Static definition of a quest: what it's called and which objectives must all be
    /// satisfied to complete it. Per-player progress (QuestState, objective counters) lives
    /// in whatever IQuestService implementation a project provides — this asset is just the
    /// design-time data, not runtime state.
    /// </summary>
    [CreateAssetMenu(fileName = "NewQuestData", menuName = "55HP/Progression/Quest Data")]
    public class QuestData : ScriptableObject
    {
        [SerializeField] private string id = string.Empty;
        [SerializeField] private string title = string.Empty;
        [SerializeField, Multiline] private string description = string.Empty;
        [SerializeField] private QuestObjective[] objectives = null;

        public string Id => id;
        public string Title => title;
        public string Description => description;
        public QuestObjective[] Objectives => objectives;
    }
}
