using System.Collections.Generic;
using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// Server-authoritative economy — currencies/items/virtual purchases defined in the UGS
    /// dashboard (cloud-side config), not Inspector ScriptableObjects/enums. Deliberately does
    /// NOT implement or replace IInventoryService/ICurrencyService, for the same reason
    /// ICloudSaveService doesn't implement/replace ISaveService (see ICloudSaveService's doc
    /// comment for the full argument — sync/local-authority vs. async/server-authority is a
    /// load-bearing difference, not incidental, so it isn't restated here).
    ///
    /// Currency ids are UGS-configured strings, not this template's CurrencyType enum — UGS
    /// currencies aren't fixed to Soft/Hard/Premium. A concrete project decides per-currency/
    /// per-item whether it's UGS-authoritative (goes through this interface) or purely local
    /// (goes through IInventoryService/ICurrencyService) — this template doesn't force one
    /// model over the other.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() and may legitimately be absent at
    /// runtime — same UGS-init-can-fail reasoning as IAuthenticationService/ICloudSaveService.
    /// Resolve with ServiceRegistry.TryResolve&lt;IEconomyService&gt;(), not Resolve&lt;T&gt;().
    /// </summary>
    public interface IEconomyService
    {
        /// <summary>Currency id (UGS-configured) → balance.</summary>
        Task<Dictionary<string, long>> GetBalancesAsync();

        /// <summary>Server validates cost/inventory atomically — this either fully succeeds
        /// or fully fails, there is no partial/client-side fallback.</summary>
        Task<bool> TryPurchaseVirtualAsync(string virtualPurchaseId);

        /// <summary>Item ids (UGS-configured) the player currently owns.</summary>
        Task<List<string>> GetInventoryAsync();
    }
}
