using System.Collections.Generic;
using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Social
{
    /// <summary>
    /// Contract for leaderboard submission/reads. Greenfield — no existing template seam to
    /// reconcile with (unlike IEconomyService vs. IInventoryService/ICurrencyService). Placed
    /// under a new Social/ folder rather than Progression/: leaderboards are a competitive/
    /// social feature, not an inventory/currency/upgrade/quest concern, and warrant their own
    /// folder the same way Combat/AI/Camera/Movement each got their own rather than being
    /// lumped into Gameplay/.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() and may legitimately be absent at
    /// runtime — same UGS-init-can-fail reasoning as every other UGS-backed service in this
    /// template. Resolve with ServiceRegistry.TryResolve&lt;ILeaderboardService&gt;(), not
    /// Resolve&lt;T&gt;().
    /// </summary>
    public interface ILeaderboardService
    {
        Task<bool> SubmitScoreAsync(string leaderboardId, long score);

        Task<List<LeaderboardEntry>> GetTopScoresAsync(string leaderboardId, int count);

        Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId);
    }
}
