using System;

namespace hp55games.Mobile.Core.IAP
{
    /// <summary>
    /// Outcome of IIAPService.PurchaseAsync. receipt is the raw, unvalidated receipt string —
    /// do NOT attempt receipt validation client-side; that belongs server-side (e.g. a UGS
    /// Cloud Code module calling the App Store/Play Store server APIs), out of scope for this
    /// template. A client can be tampered with, so a client-side "this receipt looks valid"
    /// check provides no real security — only a server call the client can't control does.
    /// </summary>
    [Serializable]
    public class PurchaseResult
    {
        public bool success;
        public string productId;
        public string receipt;
    }
}
