using System.Collections.Generic;
using System.Threading.Tasks;

namespace hp55games.Mobile.Core.IAP
{
    /// <summary>
    /// Contract for real-money purchases through the platform store (Apple/Google) — a
    /// different concern from IEconomyService (Progression/), which models UGS Economy's
    /// virtual purchases (spending a server-authoritative virtual currency/item). A rewarded-
    /// ad-for-soft-currency flow goes through IAdsService + IEconomyService/ICurrencyService;
    /// a $4.99 gem pack goes through this interface. They can compose — IAP grants currency
    /// that then flows through IEconomyService/ICurrencyService — but IAP itself is platform-
    /// store-authoritative, not UGS-authoritative, so it gets its own interface rather than
    /// being folded into IEconomyService.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() — the product catalog is
    /// per-project.
    /// </summary>
    public interface IIAPService
    {
        /// <summary>Must be called with the project's product definitions before any
        /// purchase call works — Unity IAP requires products declared up front.</summary>
        Task InitializeAsync(ProductCatalogItem[] products);

        Task<PurchaseResult> PurchaseAsync(string productId);

        bool IsInitialized { get; }

        List<ProductCatalogItem> AvailableProducts { get; }
    }
}
