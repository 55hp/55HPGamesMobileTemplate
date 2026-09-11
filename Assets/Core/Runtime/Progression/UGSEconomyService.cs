using System.Collections.Generic;
using System.Threading.Tasks;
using UGSEconomy = Unity.Services.Economy;

namespace hp55games.Mobile.Core.Progression
{
    /// <summary>
    /// IEconomyService backed by Unity.Services.Economy.EconomyService.
    ///
    /// Imported via an alias (UGSEconomy), not a plain "using Unity.Services.Economy;" — that
    /// namespace declares its own Unity.Services.Economy.IEconomyService, identically named
    /// to this template's. Same collision shape as Phase 2's UGSAuthenticationService/
    /// UGSCloudSaveService vs. Unity.Services.Authentication/CloudSave's own identically-named
    /// interfaces — avoided the same way, by not importing the colliding name unqualified.
    ///
    /// GetBalancesAsync/GetInventoryAsync fetch a single page of up to 100 entries (the UGS
    /// API's own per-call max) rather than following full pagination (HasNext) — a minimal
    /// implementation matching this template's own "don't over-engineer beyond this phase's
    /// scope" rule (same call as Phase 2's SetCategoryVolume). A project with more than 100
    /// currencies/items needs to extend this with real pagination.
    ///
    /// Constructed and registered only after UnityServices.InitializeAsync() has completed
    /// successfully — see GameBootstrap.InitializeUgsCoroutine() — never registered
    /// synchronously inside ServiceRegistry.InstallDefaults().
    /// </summary>
    public sealed class UGSEconomyService : IEconomyService
    {
        private const int MaxPageSize = 100;

        public async Task<Dictionary<string, long>> GetBalancesAsync()
        {
            var options = new UGSEconomy.PlayerBalances.GetBalancesOptions { ItemsPerFetch = MaxPageSize };
            var result = await UGSEconomy.EconomyService.Instance.PlayerBalances.GetBalancesAsync(options);

            var balances = new Dictionary<string, long>();
            foreach (var balance in result.Balances)
                balances[balance.CurrencyId] = balance.Balance;

            return balances;
        }

        public async Task<bool> TryPurchaseVirtualAsync(string virtualPurchaseId)
        {
            try
            {
                await UGSEconomy.EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(virtualPurchaseId);
                return true;
            }
            catch (UGSEconomy.EconomyException)
            {
                // Covers validation failures (cost/inventory not met) and rate limiting —
                // server rejected the purchase, nothing was charged.
                return false;
            }
            catch (Unity.Services.Core.RequestFailedException)
            {
                return false;
            }
        }

        public async Task<List<string>> GetInventoryAsync()
        {
            var options = new UGSEconomy.PlayerInventory.GetInventoryOptions { ItemsPerFetch = MaxPageSize };
            var result = await UGSEconomy.EconomyService.Instance.PlayerInventory.GetInventoryAsync(options);

            var itemIds = new List<string>();
            foreach (var item in result.PlayersInventoryItems)
                itemIds.Add(item.InventoryItemId);

            return itemIds;
        }
    }
}
