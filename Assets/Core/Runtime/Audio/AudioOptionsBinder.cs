using System;
using UnityEngine;
using UnityEngine.Audio;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Audio
{
    /// <summary>
    /// Applica Music/SFX da IUIOptionsService ai parametri esposti dell'AudioMixer.
    /// Aggancia questo a un GO in 90_Systems_Audio e assegna il Mixer.
    /// </summary>
    public sealed class AudioOptionsBinder : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private string musicParam = "MusicVol";
        [SerializeField] private string sfxParam   = "SfxVol";

        private IUIOptionsService _opt;

        private void Awake()
        {
            if (!ServiceRegistry.TryResolve<IUIOptionsService>(out _opt))
            {
                Debug.LogError("[AudioOptionsBinder] IUIOptionsService non registrato. Controlla ServiceRegistry.InstallDefaults().");
                enabled = false;
                return;
            }

            _opt.Load(); // assicura stato pronto
            _opt.Changed += ApplyAll;
        }

        private void Start()
        {
            ApplyAll();
        }

        private void OnDestroy()
        {
            if (_opt != null) _opt.Changed -= ApplyAll;
        }

        private void ApplyAll()
        {
            var musicDb = _opt.MusicMute ? -80f : Linear01ToDb(_opt.MusicVolume);
            var sfxDb   = _opt.SfxMute   ? -80f : Linear01ToDb(_opt.SfxVolume);

            mixer.SetFloat(musicParam, musicDb);
            mixer.SetFloat(sfxParam,   sfxDb);
        }


        private static float Linear01ToDb(float x)
        {
            // Evita -∞: usa -80 dB come "mute"
            if (x <= 0.0001f) return -80f;
            return Mathf.Log10(Mathf.Clamp01(x)) * 20f; // 1 -> 0 dB, 0.5 -> ~-6 dB
        }
    }
}