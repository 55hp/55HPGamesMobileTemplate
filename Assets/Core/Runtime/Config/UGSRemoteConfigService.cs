using System.Threading.Tasks;
using Unity.Services.RemoteConfig;

namespace hp55games.Mobile.Core.Config
{
    /// <summary>
    /// IRemoteConfigService backed by Unity.Services.RemoteConfig.RemoteConfigService.
    ///
    /// Collision check (per the dossier's explicit warning not to assume this one is clean
    /// just because it's last): Unity.Services.RemoteConfig's top-level members are
    /// ConfigManager, RemoteConfigService, RuntimeConfig, ConfigResponse, IRCUnityWebRequest,
    /// ConfigOrigin, ConfigRequestStatus — no IRemoteConfigService, no clash with this
    /// template's interface. A plain "using Unity.Services.RemoteConfig;" is safe here,
    /// unlike Phase 2/3's Authentication/CloudSave/Economy adapters.
    ///
    /// FetchConfigsAsync requires generic user/app attribute structs even when a project has
    /// none to target by — EmptyAttributes is an empty placeholder for exactly that case; a
    /// concrete project wanting targeted config rules would pass its own attribute structs
    /// instead of this class's signature, which is out of scope for this generic adapter.
    ///
    /// Constructed and registered only after UnityServices.InitializeAsync() has completed
    /// successfully — see GameBootstrap.InitializeUgsCoroutine() — never registered
    /// synchronously inside ServiceRegistry.InstallDefaults().
    /// </summary>
    public sealed class UGSRemoteConfigService : IRemoteConfigService
    {
        private struct EmptyAttributes { }

        private RuntimeConfig _config;

        public async Task FetchAsync()
        {
            _config = await RemoteConfigService.Instance.FetchConfigsAsync(new EmptyAttributes(), new EmptyAttributes());
        }

        public bool GetBool(string key, bool defaultValue = false) =>
            _config != null ? _config.GetBool(key, defaultValue) : defaultValue;

        public int GetInt(string key, int defaultValue = 0) =>
            _config != null ? _config.GetInt(key, defaultValue) : defaultValue;

        public float GetFloat(string key, float defaultValue = 0f) =>
            _config != null ? _config.GetFloat(key, defaultValue) : defaultValue;

        public string GetString(string key, string defaultValue = "") =>
            _config != null ? _config.GetString(key, defaultValue) : defaultValue;
    }
}
