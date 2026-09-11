using System.Threading.Tasks;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif

namespace hp55games.Mobile.Core.Privacy
{
    /// <summary>
    /// A real, non-placeholder default — unlike NoOpAdsService/DebugLogAnalyticsService,
    /// which are inert. On iOS this wraps Apple's AppTrackingTransparency prompt via
    /// com.unity.ads.ios-support, because ATT is an OS-level gate every iOS app needs,
    /// regardless of project. On Android/elsewhere, CurrentStatus defaults to Granted: there
    /// is no OS-level tracking gate equivalent to ATT at this template's scope — a real
    /// CMP/UMP flow for GDPR/CCPA is a project-specific addition layered on top of this
    /// default, not something this class attempts to provide generically.
    ///
    /// Registered by ServiceRegistry.InstallDefaults() — a deliberate exception to "new
    /// adapters this phase aren't registered by default." Every other adapter in this dossier
    /// needs a per-project SDK key/catalog/config it can't ship; this one needs none — it's
    /// platform API only, so there's nothing project-specific blocking a default registration.
    /// </summary>
    public sealed class DefaultConsentService : IConsentService
    {
        private ConsentStatus _currentStatus = ConsentStatus.NotDetermined;

        public DefaultConsentService()
        {
#if !UNITY_IOS
            _currentStatus = ConsentStatus.Granted;
#endif
        }

        public ConsentStatus CurrentStatus => _currentStatus;

        public Task<ConsentStatus> RequestConsentAsync()
        {
#if UNITY_IOS
            var existing = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (existing != ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                _currentStatus = ToConsentStatus(existing);
                return Task.FromResult(_currentStatus);
            }

            var tcs = new TaskCompletionSource<ConsentStatus>();
            ATTrackingStatusBinding.RequestAuthorizationTracking(status =>
            {
                _currentStatus = ToConsentStatus((ATTrackingStatusBinding.AuthorizationTrackingStatus)status);
                tcs.TrySetResult(_currentStatus);
            });
            return tcs.Task;
#else
            return Task.FromResult(_currentStatus);
#endif
        }

#if UNITY_IOS
        private static ConsentStatus ToConsentStatus(ATTrackingStatusBinding.AuthorizationTrackingStatus status) =>
            status switch
            {
                ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED => ConsentStatus.Granted,
                ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED => ConsentStatus.Denied,
                ATTrackingStatusBinding.AuthorizationTrackingStatus.RESTRICTED => ConsentStatus.Restricted,
                _ => ConsentStatus.NotDetermined
            };
#endif
    }
}
