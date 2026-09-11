using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.LevelPlay;

namespace hp55games.Mobile.Core.Ads
{
    /// <summary>
    /// IAdsService backed by Unity LevelPlay (Unity.Services.LevelPlay, the current mediation
    /// SDK/package name post ironSource→LevelPlay rebrand — verified against current docs
    /// rather than assumed, since this package has renamed before). Collision-checked: the
    /// Unity.Services.LevelPlay namespace declares no IAdsService/LevelPlayAdsService, so a
    /// plain "using" is safe here, unlike the UGS adapters in this dossier.
    ///
    /// Ad objects (LevelPlayInterstitialAd/LevelPlayRewardedAd/LevelPlayBannerAd) must be
    /// created only after LevelPlay.OnInitSuccess fires, per LevelPlay's own integration
    /// docs — hence the two-phase constructor/OnInitSuccess split below rather than
    /// constructing them eagerly.
    ///
    /// IsInterstitialReady/IsRewardedReady reflect OnAdLoaded/OnAdLoadFailed/OnAdClosed state
    /// transitions, never a guess. ShowRewardedAsync resolves true only if OnAdRewarded fired
    /// before OnAdClosed — closing without a reward (skip, network drop) resolves false,
    /// exactly the contract IAdsService documents.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() — LevelPlay needs an app key and
    /// ad unit ids configured per project, same reasoning as every other real adapter so far.
    /// </summary>
    public sealed class LevelPlayAdsService : IAdsService
    {
        private readonly string _interstitialAdUnitId;
        private readonly string _rewardedAdUnitId;
        private readonly string _bannerAdUnitId;

        private LevelPlayInterstitialAd _interstitialAd;
        private LevelPlayRewardedAd _rewardedAd;
        private LevelPlayBannerAd _bannerAd;

        private bool _interstitialReady;
        private bool _rewardedReady;
        private bool _rewardGranted;

        private TaskCompletionSource<bool> _interstitialClosedTcs;
        private TaskCompletionSource<bool> _rewardedResultTcs;

        public bool IsInterstitialReady => _interstitialReady;
        public bool IsRewardedReady => _rewardedReady;

        public LevelPlayAdsService(string appKey, string interstitialAdUnitId,
            string rewardedAdUnitId, string bannerAdUnitId)
        {
            _interstitialAdUnitId = interstitialAdUnitId;
            _rewardedAdUnitId = rewardedAdUnitId;
            _bannerAdUnitId = bannerAdUnitId;

            LevelPlay.OnInitSuccess += OnInitSuccess;
            LevelPlay.OnInitFailed += OnInitFailed;
            LevelPlay.Init(appKey);
        }

        public Task ShowInterstitialAsync()
        {
            if (!_interstitialReady || _interstitialAd == null)
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<bool>();
            _interstitialClosedTcs = tcs;
            _interstitialAd.ShowAd();
            return tcs.Task;
        }

        public Task<bool> ShowRewardedAsync()
        {
            if (!_rewardedReady || _rewardedAd == null)
                return Task.FromResult(false);

            _rewardGranted = false;
            var tcs = new TaskCompletionSource<bool>();
            _rewardedResultTcs = tcs;
            _rewardedAd.ShowAd();
            return tcs.Task;
        }

        public void ShowBanner() => _bannerAd?.ShowAd();

        public void HideBanner() => _bannerAd?.HideAd();

        private void OnInitSuccess(LevelPlayConfiguration configuration)
        {
            _interstitialAd = new LevelPlayInterstitialAd(_interstitialAdUnitId);
            _interstitialAd.OnAdLoaded += OnInterstitialLoaded;
            _interstitialAd.OnAdLoadFailed += OnInterstitialLoadFailed;
            _interstitialAd.OnAdClosed += OnInterstitialClosed;
            _interstitialAd.LoadAd();

            _rewardedAd = new LevelPlayRewardedAd(_rewardedAdUnitId);
            _rewardedAd.OnAdLoaded += OnRewardedLoaded;
            _rewardedAd.OnAdLoadFailed += OnRewardedLoadFailed;
            _rewardedAd.OnAdRewarded += OnRewardedGranted;
            _rewardedAd.OnAdClosed += OnRewardedClosed;
            _rewardedAd.LoadAd();

            _bannerAd = new LevelPlayBannerAd(_bannerAdUnitId);
        }

        private void OnInitFailed(LevelPlayInitError error)
        {
            Debug.LogError($"[LevelPlayAdsService] LevelPlay.Init failed: {error}");
        }

        private void OnInterstitialLoaded(LevelPlayAdInfo info) => _interstitialReady = true;

        private void OnInterstitialLoadFailed(LevelPlayAdError error) => _interstitialReady = false;

        private void OnInterstitialClosed(LevelPlayAdInfo info)
        {
            _interstitialReady = false;
            _interstitialClosedTcs?.TrySetResult(true);
            _interstitialClosedTcs = null;

            // Preload the next one now rather than waiting for the next ShowInterstitialAsync
            // call to discover there's nothing ready.
            _interstitialAd.LoadAd();
        }

        private void OnRewardedLoaded(LevelPlayAdInfo info) => _rewardedReady = true;

        private void OnRewardedLoadFailed(LevelPlayAdError error) => _rewardedReady = false;

        private void OnRewardedGranted(LevelPlayAdInfo info, LevelPlayReward reward) => _rewardGranted = true;

        private void OnRewardedClosed(LevelPlayAdInfo info)
        {
            _rewardedReady = false;
            _rewardedResultTcs?.TrySetResult(_rewardGranted);
            _rewardedResultTcs = null;

            _rewardedAd.LoadAd();
        }
    }
}
