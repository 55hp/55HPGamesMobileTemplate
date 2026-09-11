using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Privacy
{
    /// <summary>
    /// Contract for the tracking-consent flow (ATT on iOS, a CMP/UMP flow elsewhere) —
    /// GDPR/CCPA/COPPA/ATT make this a legal requirement, not an optional nicety.
    ///
    /// This is a GATE, not a formality: Analytics/Ads/anything that tracks the user must not
    /// initialize before RequestConsentAsync() resolves. This interface does not and cannot
    /// enforce that ordering itself — it doesn't own Analytics/Ads initialization — so every
    /// implementation and every call site that wires tracking SDKs together is responsible
    /// for gating on this explicitly. Getting this wrong is a compliance violation, not a bug
    /// that only shows up as incorrect behavior.
    /// </summary>
    public interface IConsentService
    {
        Task<ConsentStatus> RequestConsentAsync();

        ConsentStatus CurrentStatus { get; }
    }
}
