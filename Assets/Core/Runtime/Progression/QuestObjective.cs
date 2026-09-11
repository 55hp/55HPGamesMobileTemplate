using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// One countable condition inside a QuestData. Not a ScriptableObject — an objective only
    /// ever exists nested inside a single quest's objectives array, so it doesn't need its own
    /// asset identity; [Serializable] is enough for the Inspector to edit it inline.
    /// </summary>
    [Serializable]
    public class QuestObjective
    {
        [Tooltip("Matches the eventType param IQuestService.ReportEvent is called with.")]
        [SerializeField] private string requiredEventType = string.Empty;

        [SerializeField] private int requiredCount = 1;

        public string RequiredEventType => requiredEventType;
        public int RequiredCount => requiredCount;
    }
}
