using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Config
{
    /// <summary>
    /// Contract for UGS Remote Config. Greenfield, no existing template seam to reconcile
    /// with. Placed alongside IConfigService/GameConfig in Config/ rather than Progression/ or
    /// a new folder — both are "where does a value come from" concerns, just local
    /// Addressables-loaded config vs. cloud-fetched config.
    ///
    /// Values are unavailable (return their defaultValue) until FetchAsync() has completed at
    /// least once — callers must not assume a value is live before that. This is deliberate,
    /// not a missing-loading-state bug: a default-returning getter lets call sites read
    /// config defensively without null-checking an "IsReady" flag themselves.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() and may legitimately be absent at
    /// runtime — same UGS-init-can-fail reasoning as every other UGS-backed service in this
    /// template. Resolve with ServiceRegistry.TryResolve&lt;IRemoteConfigService&gt;(), not
    /// Resolve&lt;T&gt;().
    /// </summary>
    public interface IRemoteConfigService
    {
        Task FetchAsync();

        bool GetBool(string key, bool defaultValue = false);

        int GetInt(string key, int defaultValue = 0);

        float GetFloat(string key, float defaultValue = 0f);

        string GetString(string key, string defaultValue = "");
    }
}
