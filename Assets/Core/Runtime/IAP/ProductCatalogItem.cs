using System;

namespace hp55games.Mobile.Core.IAP
{
    /// <summary>Which platform-store purchase model a product uses.</summary>
    public enum ProductType
    {
        Consumable,
        NonConsumable,
        Subscription
    }

    /// <summary>
    /// Static definition of a store product, fed into IIAPService.InitializeAsync up front —
    /// Unity IAP requires product definitions before any purchase call works. Not a
    /// ScriptableObject: product ids/types are typically defined alongside store console
    /// config (App Store Connect / Google Play Console), so this is populated in code or from
    /// whatever catalog source a project already has, not authored as a template asset.
    ///
    /// localizedPriceString is populated only after the store connection completes (it comes
    /// back from the platform store, not this definition) — read it from
    /// IIAPService.AvailableProducts after InitializeAsync finishes, not from an instance you
    /// constructed yourself to pass into InitializeAsync.
    /// </summary>
    [Serializable]
    public class ProductCatalogItem
    {
        public string productId;
        public ProductType productType;
        public string localizedPriceString;
    }
}
