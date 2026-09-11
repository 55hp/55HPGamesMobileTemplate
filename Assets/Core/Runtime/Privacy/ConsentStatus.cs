namespace hp55games.Mobile.Core.Privacy
{
    /// <summary>
    /// Matches Apple's AppTrackingTransparency status shape deliberately — ATT is the
    /// strictest of the platforms this template needs to satisfy (it's an OS-level gate with
    /// no way around it, unlike GDPR/CCPA which are legal requirements a project implements
    /// via its own CMP/UMP flow). Other platforms' consent states map onto this same set.
    /// </summary>
    public enum ConsentStatus
    {
        NotDetermined,
        Granted,
        Denied,
        Restricted
    }
}
