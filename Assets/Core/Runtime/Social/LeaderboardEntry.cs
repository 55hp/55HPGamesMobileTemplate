using System;

namespace hp55games.Mobile.Core.Social
{
    /// <summary>
    /// One row of a leaderboard. Not a ScriptableObject — it's runtime data returned by
    /// ILeaderboardService, never designer-authored, same reasoning as DialogueLineData/
    /// QuestObjective in earlier phases.
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public string playerId;
        public string playerName;
        public long score;
        public int rank;
    }
}
