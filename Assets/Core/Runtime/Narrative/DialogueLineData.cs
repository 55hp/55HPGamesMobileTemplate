using System;
using UnityEngine;

namespace hp55games.Mobile.Core.Narrative
{
    /// <summary>
    /// One line of dialogue. Not a ScriptableObject — same reasoning as Phase 4's
    /// QuestObjective: a line only ever exists nested inside whatever authors a dialogue
    /// (e.g. an array literal passed straight into IDialogueService.StartDialogue), so it
    /// doesn't need its own asset identity. [Serializable] is enough for inline Inspector editing.
    /// </summary>
    [Serializable]
    public class DialogueLineData
    {
        [SerializeField] private string speakerName = string.Empty;

        [SerializeField, Multiline] private string text = string.Empty;

        [Tooltip("Plain choice labels. SelectChoice(int) on IDialogueService takes the index " +
                 "into this array. Empty/null means this line has no branching choices.")]
        [SerializeField] private string[] choices = null;

        public string SpeakerName => speakerName;
        public string Text => text;
        public string[] Choices => choices;
    }
}
