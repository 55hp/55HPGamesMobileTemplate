using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using hp55games.Mobile.Core.Config; // per GameConfig

namespace hp55games.Mobile.Core.Architecture
{
    public interface IConfigService
    {
        GameConfig Current { get; }
        bool IsReady { get; }
        Task InitializeAsync();
        void Release();
    }

    /// <summary>
    /// Carica e mantiene in memoria la GameConfig via Addressables.
    /// - Chiave usata: "config/main"
    /// - Chiamare InitializeAsync() all’avvio (bootstrap) PRIMA di usare Current.
    /// </summary>
    public sealed class ConfigService : IConfigService
    {
        private const string ADDRESS = "config/main";

        private AsyncOperationHandle<GameConfig>? _handle;
        private Task _initTask; // evita doppie inizializzazioni concorrenti

        public GameConfig Current { get; private set; }
        public bool IsReady => Current != null;

        public Task InitializeAsync()
        {
            // evita doppio caricamento se qualcuno richiama InitializeAsync più volte
            if (_initTask != null) return _initTask;

            _initTask = InitializeInternalAsync();
            return _initTask;
        }

        private async Task InitializeInternalAsync()
        {
            if (IsReady) return;

            AsyncOperationHandle<GameConfig> handle = Addressables.LoadAssetAsync<GameConfig>(ADDRESS);

            try
            {
                var cfg = await handle.Task; // attende caricamento
                if (cfg == null)
                {
                    Debug.LogError($"[ConfigService] Config null per address '{ADDRESS}'. " +
                                   $"Controlla Address e Build Player Content.");
                    return;
                }

                _handle = handle;
                Current = cfg;

#if UNITY_EDITOR
                Debug.Log($"[ConfigService] Caricata config '{ADDRESS}' " +
                          $"(AppVersion={cfg.appVersion}, Haptics={cfg.enableHaptics}, Diff={cfg.defaultDifficulty})");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                // Se fallisce, rilascia handle parziale
                if (handle.IsValid()) Addressables.Release(handle);
                _handle = null;
                Current = null;
            }
        }

        public void Release()
        {
            if (_handle.HasValue)
            {
                Addressables.Release(_handle.Value);
                _handle = null;
            }
            Current = null;
            _initTask = null;
        }
    }
}
