using System.Collections.Generic;
using System.Threading.Tasks;
using UGSCloudSave = Unity.Services.CloudSave;

namespace hp55games.Mobile.Core.Save
{
    /// <summary>
    /// ICloudSaveService backed by Unity.Services.CloudSave.CloudSaveService.
    ///
    /// Imported via an alias (UGSCloudSave), not a plain "using Unity.Services.CloudSave;" —
    /// that namespace declares its own Unity.Services.CloudSave.ICloudSaveService, identically
    /// named to this template's ICloudSaveService. Same collision shape as
    /// UGSAuthenticationService vs. Unity.Services.Authentication.IAuthenticationService —
    /// avoided by not importing the colliding name unqualified.
    ///
    /// IsAvailable() is a lightweight readiness check (UGS initialized + player signed in),
    /// not an actual network round-trip — probing real reachability would need a live network
    /// call, which isn't worth the cost just to answer "can I try a sync right now".
    ///
    /// Constructed and registered only after UnityServices.InitializeAsync() has completed
    /// successfully — see GameBootstrap.BootstrapSequence() — never registered synchronously
    /// inside ServiceRegistry.InstallDefaults().
    /// </summary>
    public sealed class UGSCloudSaveService : ICloudSaveService
    {
        public async Task<bool> SyncUpAsync(string key, string jsonValue)
        {
            try
            {
                var data = new Dictionary<string, object> { [key] = jsonValue };
                await UGSCloudSave.CloudSaveService.Instance.Data.Player.SaveAsync(data);
                return true;
            }
            catch (Unity.Services.Core.RequestFailedException)
            {
                return false;
            }
        }

        public async Task<string> SyncDownAsync(string key)
        {
            try
            {
                var keys = new HashSet<string> { key };
                var result = await UGSCloudSave.CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (result.TryGetValue(key, out var item))
                    return item.Value.GetAs<string>();

                return null;
            }
            catch (Unity.Services.Core.RequestFailedException)
            {
                return null;
            }
        }

        public Task<bool> IsAvailable()
        {
            bool initialized = Unity.Services.Core.UnityServices.State ==
                                Unity.Services.Core.ServicesInitializationState.Initialized;
            bool signedIn = Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn;

            return Task.FromResult(initialized && signedIn);
        }
    }
}
