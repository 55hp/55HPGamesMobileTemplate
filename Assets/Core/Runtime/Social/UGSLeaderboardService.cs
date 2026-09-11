using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;

namespace hp55games.Mobile.Core.Social
{
    /// <summary>
    /// ILeaderboardService backed by Unity.Services.Leaderboards.LeaderboardsService.
    ///
    /// Collision check (per the dossier's standing caution, same as the other three UGS
    /// adapters): Unity.Services.Leaderboards declares ILeaderboardsService (plural) at its
    /// top level — distinct from this template's ILeaderboardService (singular), no clash —
    /// and a colliding LeaderboardEntry type, but that one lives in the nested
    /// Unity.Services.Leaderboards.Models namespace, which this file never imports (SDK
    /// results are read via "var" and mapped into this template's LeaderboardEntry field-by-
    /// field, never by naming the SDK's type). A plain "using Unity.Services.Leaderboards;" is
    /// therefore safe here — unlike Phase 2/3's Authentication/CloudSave/Economy adapters,
    /// which needed an alias because their collision was at the top level.
    ///
    /// Constructed and registered only after UnityServices.InitializeAsync() has completed
    /// successfully — see GameBootstrap.InitializeUgsCoroutine() — never registered
    /// synchronously inside ServiceRegistry.InstallDefaults().
    /// </summary>
    public sealed class UGSLeaderboardService : ILeaderboardService
    {
        public async Task<bool> SubmitScoreAsync(string leaderboardId, long score)
        {
            try
            {
                await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, (double)score);
                return true;
            }
            catch (Unity.Services.Leaderboards.Exceptions.LeaderboardsException)
            {
                return false;
            }
            catch (Unity.Services.Core.RequestFailedException)
            {
                return false;
            }
        }

        public async Task<List<LeaderboardEntry>> GetTopScoresAsync(string leaderboardId, int count)
        {
            var options = new GetScoresOptions { Limit = count };
            var page = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, options);

            var entries = new List<LeaderboardEntry>();
            foreach (var result in page.Results)
                entries.Add(ToEntry(result.PlayerId, result.PlayerName, result.Score, result.Rank));

            return entries;
        }

        public async Task<LeaderboardEntry> GetPlayerScoreAsync(string leaderboardId)
        {
            try
            {
                var result = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);
                return ToEntry(result.PlayerId, result.PlayerName, result.Score, result.Rank);
            }
            catch (Unity.Services.Leaderboards.Exceptions.LeaderboardsException)
            {
                // Thrown when the player hasn't submitted a score yet — not an error state
                // worth propagating, the caller just has no entry to show.
                return null;
            }
            catch (Unity.Services.Core.RequestFailedException)
            {
                return null;
            }
        }

        private static LeaderboardEntry ToEntry(string playerId, string playerName, double score, int rank) =>
            new LeaderboardEntry
            {
                playerId = playerId,
                playerName = playerName,
                score = (long)score,
                rank = rank
            };
    }
}
