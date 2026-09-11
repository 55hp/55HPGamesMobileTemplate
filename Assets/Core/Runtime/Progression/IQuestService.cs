using System;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Contract for quest progress tracking. ReportEvent takes a free-form eventType string
    /// rather than a typed event object so gameplay code doesn't need a compile-time reference
    /// to every quest objective it might be contributing to — it just reports what happened
    /// ("enemy_killed", "coin_collected", ...) and objectives that care about that string react.
    /// Pure interface, no base class — not registered by ServiceRegistry.InstallDefaults();
    /// a concrete project implements and registers its own.
    /// </summary>
    public interface IQuestService
    {
        void StartQuest(QuestData quest);

        void ReportEvent(string eventType, int count = 1);

        QuestState GetQuestState(QuestData quest);

        event Action<QuestData> OnQuestCompleted;
    }
}
