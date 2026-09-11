using System;

namespace hp55games.Mobile.Core.Narrative
{
    /// <summary>
    /// Contract for driving a dialogue sequence. Pure interface, no base class — dialogue
    /// UI presentation, pacing (auto-advance vs. tap-to-advance), and branching logic vary
    /// too much per project to share a MonoBehaviour skeleton (same reasoning as Phase 4's
    /// progression services). ILocalizationService is deliberately not referenced here —
    /// text localization for dialogue is a future integration point, out of scope this phase.
    /// </summary>
    public interface IDialogueService
    {
        void StartDialogue(DialogueLineData[] lines);

        void AdvanceLine();

        void SelectChoice(int choiceIndex);

        DialogueLineData CurrentLine { get; }

        event Action OnDialogueEnded;
    }
}
