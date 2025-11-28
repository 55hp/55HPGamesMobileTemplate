using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.UI
{
    /// <summary>
    /// Gestore BGM basato su due AudioSource (A/B) per crossfade.
    /// Cerca un GameObject chiamato "MusicPlayer" in scena con DUE AudioSource.
    /// Le tracce sono AudioClip caricati via Addressables (string address).
    /// </summary>
    public sealed class UIMusicService : IMusicService
    {
        public string CurrentTrackAddress { get; private set; }
        public bool IsPlaying => _activeSource != null && _activeSource.isPlaying;

        private AudioSource _sourceA;
        private AudioSource _sourceB;
        private AudioSource _activeSource;   // attualmente “in primo piano”
        private AudioSource _inactiveSource; // usata per la prossima traccia

        private readonly Dictionary<string, AudioClip> _clipCache = new();

        private bool _missingLogged; // per non spammare errori

        const float MAX_VOLUME = 1f;

        public UIMusicService()
        {
            // niente Find qui: la scena audio potrebbe non essere ancora caricata
        }

        public async Task PlayAsync(string address, float fadeIn = 0.5f)
        {
            if (!EnsureSources()) return;

            var clip = await GetClipAsync(address);
            if (clip == null) return;

            // stop immediato della traccia precedente
            _activeSource.Stop();
            _inactiveSource.Stop();

            _activeSource.clip = clip;
            _activeSource.volume = 0f;
            _activeSource.Play();
            CurrentTrackAddress = address;

            await FadeVolumeAsync(_activeSource, 0f, MAX_VOLUME, fadeIn);
        }

        public async Task CrossfadeToAsync(string address, float duration = 0.75f)
        {
            if (!EnsureSources()) return;

            var clip = await GetClipAsync(address);
            if (clip == null) return;

            // prepara la sorgente inattiva con la nuova traccia
            _inactiveSource.clip = clip;
            _inactiveSource.volume = 0f;
            _inactiveSource.Play();
            CurrentTrackAddress = address;

            // fade: active 1->0, inactive 0->1
            float time = 0f;
            float startActiveVol = _activeSource.volume;
            float startInactiveVol = _inactiveSource.volume;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);

                _activeSource.volume   = Mathf.Lerp(startActiveVol, 0f, t);
                _inactiveSource.volume = Mathf.Lerp(startInactiveVol, MAX_VOLUME, t);

                await Task.Yield();
            }

            _activeSource.volume = 0f;
            _activeSource.Stop();
            _inactiveSource.volume = MAX_VOLUME;

            // swap A/B
            (_activeSource, _inactiveSource) = (_inactiveSource, _activeSource);
        }

        public async Task StopAsync(float fadeOut = 0.5f)
        {
            if (!EnsureSources()) return;
            if (!_activeSource.isPlaying) return;

            await FadeVolumeAsync(_activeSource, _activeSource.volume, 0f, fadeOut);
            _activeSource.Stop();
            CurrentTrackAddress = null;
        }

        // ---------- helpers ----------

        private bool EnsureSources()
        {
            // se già inizializzati, ok
            if (_sourceA != null && _sourceB != null && _activeSource != null && _inactiveSource != null)
                return true;

            // prova a trovare MusicPlayer in scena (dopo che tutte le additive sono state caricate)
            var player = GameObject.Find("MusicPlayer");
            if (player == null)
            {
                if (!_missingLogged)
                {
                    Debug.LogError("[UIMusicService] GameObject 'MusicPlayer' non trovato. Crealo in 90_Systems_Audio con due AudioSource.");
                    _missingLogged = true;
                }
                return false;
            }

            var sources = player.GetComponents<AudioSource>();
            if (sources.Length < 2)
            {
                if (!_missingLogged)
                {
                    Debug.LogError("[UIMusicService] 'MusicPlayer' deve avere almeno 2 AudioSource.");
                    _missingLogged = true;
                }
                return false;
            }

            _sourceA = sources[0];
            _sourceB = sources[1];

            _sourceA.loop = true;
            _sourceB.loop = true;

            _activeSource = _sourceA;
            _inactiveSource = _sourceB;

            _missingLogged = false;
            return true;
        }

        private async Task<AudioClip> GetClipAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogError("[UIMusicService] address nullo/vuoto.");
                return null;
            }

            if (_clipCache.TryGetValue(address, out var cached))
                return cached;

            var handle = Addressables.LoadAssetAsync<AudioClip>(address);
            await handle.Task;

            if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"[UIMusicService] Impossibile caricare AudioClip: '{address}'");
                return null;
            }

            var clip = handle.Result;
            _clipCache[address] = clip;
            return clip;
        }

        private static async Task FadeVolumeAsync(AudioSource src, float from, float to, float duration)
        {
            if (src == null) return;
            if (duration <= 0f)
            {
                src.volume = to;
                return;
            }

            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                src.volume = Mathf.Lerp(from, to, t);
                await Task.Yield();
            }
            src.volume = to;
        }
    }
}