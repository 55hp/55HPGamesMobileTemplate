using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Ads
{
    /// <summary>
    /// Mediation-agnostic ad surface: interstitial, rewarded, banner. Gameplay/UI code
    /// depends only on this interface, never on a specific SDK (AdMob, Unity LevelPlay,
    /// ...) — see NoOpAdsService for the template's default implementation.
    /// </summary>
    public interface IAdsService
    {
        /// <summary>True once an interstitial has finished preloading and is ready to show.</summary>
        bool IsInterstitialReady { get; }

        /// <summary>True once a rewarded ad has finished preloading and is ready to show.</summary>
        bool IsRewardedReady { get; }

        /// <summary>Shows an interstitial if one is ready; no-ops otherwise. Completes when the ad is dismissed.</summary>
        Task ShowInterstitialAsync();

        /// <summary>
        /// Shows a rewarded ad if one is ready. Returns true only if the player watched it
        /// to completion and the reward should be granted; false if unavailable, skipped,
        /// or closed early.
        /// </summary>
        Task<bool> ShowRewardedAsync();

        /// <summary>Shows the banner ad (typically anchored top or bottom), if configured.</summary>
        void ShowBanner();

        /// <summary>Hides the banner ad without destroying it.</summary>
        void HideBanner();
    }
}
