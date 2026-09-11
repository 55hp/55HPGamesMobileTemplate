using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Save
{
    /// <summary>
    /// Cross-device cloud sync. Deliberately separate from ISaveService, not a replacement
    /// for it: ISaveService is synchronous and local-only (System.IO, no network), and every
    /// existing caller depends on that synchronous contract. UGS Cloud Save is inherently
    /// async and network-bound — implementing ISaveService on top of it would break that
    /// contract for every caller, not just new ones. A concrete project composes both:
    /// ISaveService for offline continuity (always available, instant, no round-trip),
    /// ICloudSaveService layered on top for cross-device restore (only available when signed
    /// in and online). Don't try to unify these into one interface later — the sync/async
    /// split is load-bearing, not incidental.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() and may legitimately be absent at
    /// runtime — UGS init can fail (no network, UGS project not linked for this Unity
    /// project). Resolve with ServiceRegistry.TryResolve&lt;ICloudSaveService&gt;(), not
    /// Resolve&lt;T&gt;(), since unlike every other default service it may not be there.
    /// </summary>
    public interface ICloudSaveService
    {
        /// <summary>Pushes local data to the cloud under the given key.</summary>
        Task<bool> SyncUpAsync(string key, string jsonValue);

        /// <summary>Pulls cloud data for the given key. Returns null if absent.</summary>
        Task<string> SyncDownAsync(string key);

        Task<bool> IsAvailable();
    }
}
