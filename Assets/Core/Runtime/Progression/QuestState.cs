namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Per-player status for a given QuestData. Separate from whether a quest is merely
    /// "known to exist" — a quest not yet started is NotStarted, not absent.
    /// </summary>
    public enum QuestState
    {
        NotStarted,
        Active,
        Completed,
        Failed
    }
}
