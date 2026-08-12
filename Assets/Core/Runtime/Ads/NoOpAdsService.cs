using System.Threading.Tasks;
using UnityEngine;

namespace hp55games.Mobile.Core.Ads
{
    /// <summary>
    /// Default IAdsService: no ad SDK is integrated in the template, so every call logs
    /// what would have happened and returns immediately. IsInterstitialReady/IsRewardedReady
    /// are always false and ShowRewardedAsync never grants a reward, so UI built against
    /// this contract (e.g. hiding a "watch ad" button when not ready) behaves correctly
    /// with zero ads wired up. Swap this registration in ServiceRegistry.InstallDefaults()
    /// for a real adapter (AdMob, Unity LevelPlay, ...) once one is integrated — no
    /// gameplay/UI call site needs to change.
    /// </summary>
    public sealed class NoOpAdsService : IAdsService
    {
        public bool IsInterstitialReady => false;
        public bool IsRewardedReady => false;

        public Task ShowInterstitialAsync()
        {
            Debug.Log("[NoOpAdsService] ShowInterstitialAsync called, no ad SDK integrated.");
            return Task.CompletedTask;
        }

        public Task<bool> ShowRewardedAsync()
        {
            Debug.Log("[NoOpAdsService] ShowRewardedAsync called, no ad SDK integrated. Reward NOT granted.");
            return Task.FromResult(false);
        }

        public void ShowBanner() => Debug.Log("[NoOpAdsService] ShowBanner called, no ad SDK integrated.");
        public void HideBanner() => Debug.Log("[NoOpAdsService] HideBanner called, no ad SDK integrated.");
    }
}
