using System.Collections.Generic;
using System.Threading.Tasks;
using UnityIAP = UnityEngine.Purchasing;

namespace hp55games.Mobile.Core.IAP
{
    /// <summary>
    /// IIAPService backed by UnityEngine.Purchasing (Unity IAP v5's UnityIAPServices/
    /// StoreController API, not the older IStoreListener pattern).
    ///
    /// Imported via an alias (UnityIAP), not a plain "using UnityEngine.Purchasing;" — that
    /// namespace declares its own ProductCatalogItem and ProductType, both identically named
    /// to this template's (Unity IAP's ProductCatalogItem represents a store-console catalog
    /// entry for import/export tooling; an unrelated purpose, same name). A plain using would
    /// make every unqualified reference to either name ambiguous (CS0104) — same collision
    /// shape as every other UGS/SDK adapter in this dossier, resolved the same way.
    ///
    /// InitializeAsync doesn't resolve until OnProductsFetched actually fires — Connect()
    /// alone only establishes the store connection, FetchProducts() is fire-and-forget and
    /// delivers results via the event, so a TaskCompletionSource bridges that callback back
    /// into the awaitable contract this interface promises. PurchaseAsync does the same for
    /// OnPurchasePending/OnPurchaseFailed, keyed by product id.
    /// </summary>
    public sealed class UnityIAPService : IIAPService
    {
        private UnityIAP.StoreController _storeController;
        private readonly List<ProductCatalogItem> _availableProducts = new List<ProductCatalogItem>();
        private readonly Dictionary<string, TaskCompletionSource<PurchaseResult>> _pendingPurchases =
            new Dictionary<string, TaskCompletionSource<PurchaseResult>>();

        private TaskCompletionSource<bool> _productsFetchedTcs;

        public bool IsInitialized { get; private set; }

        public List<ProductCatalogItem> AvailableProducts => _availableProducts;

        public async Task InitializeAsync(ProductCatalogItem[] products)
        {
            _storeController = UnityIAP.UnityIAPServices.StoreController();
            _storeController.OnProductsFetched += OnProductsFetched;
            _storeController.OnPurchasePending += OnPurchasePending;
            _storeController.OnPurchaseFailed += OnPurchaseFailed;

            await _storeController.Connect();

            var definitions = new List<UnityIAP.ProductDefinition>();
            foreach (var item in products)
                definitions.Add(new UnityIAP.ProductDefinition(item.productId, ToSdkProductType(item.productType)));

            _productsFetchedTcs = new TaskCompletionSource<bool>();
            _storeController.FetchProducts(definitions);
            await _productsFetchedTcs.Task;

            IsInitialized = true;
        }

        public Task<PurchaseResult> PurchaseAsync(string productId)
        {
            var tcs = new TaskCompletionSource<PurchaseResult>();
            _pendingPurchases[productId] = tcs;
            _storeController.PurchaseProduct(productId);
            return tcs.Task;
        }

        private void OnProductsFetched(List<UnityIAP.Product> products)
        {
            _availableProducts.Clear();
            foreach (var product in products)
            {
                _availableProducts.Add(new ProductCatalogItem
                {
                    productId = product.definition.id,
                    productType = FromSdkProductType(product.definition.type),
                    localizedPriceString = product.metadata.localizedPriceString
                });
            }

            _productsFetchedTcs?.TrySetResult(true);
        }

        private void OnPurchasePending(UnityIAP.PendingOrder pendingOrder)
        {
            string productId = GetProductId(pendingOrder);
            string receipt = pendingOrder.Info.Receipt;

            // Confirming tells the store the app has recorded this purchase — required before
            // the platform considers it fully delivered (and, for consumables, purchasable again).
            _storeController.ConfirmPurchase(pendingOrder);

            if (productId != null && _pendingPurchases.TryGetValue(productId, out var tcs))
            {
                tcs.TrySetResult(new PurchaseResult { success = true, productId = productId, receipt = receipt });
                _pendingPurchases.Remove(productId);
            }
        }

        private void OnPurchaseFailed(UnityIAP.FailedOrder failedOrder)
        {
            string productId = GetProductId(failedOrder);
            if (productId != null && _pendingPurchases.TryGetValue(productId, out var tcs))
            {
                tcs.TrySetResult(new PurchaseResult
                {
                    success = false,
                    productId = productId,
                    receipt = null
                });
                _pendingPurchases.Remove(productId);
            }
        }

        // PendingOrder/FailedOrder carry no direct ProductId — it's on the cart item's
        // Product instead (uSku, the Unity-side catalog id), reached via the shared Order
        // base class both derive from.
        private static string GetProductId(UnityIAP.Order order)
        {
            var items = order.CartOrdered.Items();
            return items.Count > 0 ? items[0].Product.uSku : null;
        }

        private static UnityIAP.ProductType ToSdkProductType(ProductType type) => type switch
        {
            ProductType.NonConsumable => UnityIAP.ProductType.NonConsumable,
            ProductType.Subscription => UnityIAP.ProductType.Subscription,
            _ => UnityIAP.ProductType.Consumable
        };

        private static ProductType FromSdkProductType(UnityIAP.ProductType type) => type switch
        {
            UnityIAP.ProductType.NonConsumable => ProductType.NonConsumable,
            UnityIAP.ProductType.Subscription => ProductType.Subscription,
            _ => ProductType.Consumable
        };
    }
}
