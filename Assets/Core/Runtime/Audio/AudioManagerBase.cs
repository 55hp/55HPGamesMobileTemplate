using System.Collections.Generic;
using UnityEngine;

namespace hp55games.Mobile.Core.Audio
{
    /// <summary>
    /// Base SFX player backed by a fixed pool of AudioSource components rather than
    /// AudioSource.PlayClipAtPoint or one Instantiate per call — mobile-conscious, same
    /// spirit as IObjectPoolService even though pooling AudioSources doesn't need that
    /// service directly (they're components added once, not GameObjects spawned/despawned
    /// per call).
    ///
    /// When every pooled source is busy, a new SFX request is dropped rather than stealing
    /// the oldest-playing source: on mobile, a missed one-shot (footstep, hit) is far less
    /// noticeable than an audible pop from cutting another sound off mid-playback. Raise
    /// poolSize if drops become audible in practice.
    ///
    /// PlaySFX(clip, volume) — the IAudioManager member — funnels into the category-aware
    /// overload below using DefaultCategory, since the interface intentionally keeps the
    /// common call site simple (fire-and-forget, no category bookkeeping for callers that
    /// don't need it) while SetCategoryVolume still needs *some* category to multiply against.
    /// </summary>
    public abstract class AudioManagerBase : MonoBehaviour, IAudioManager
    {
        private const string DefaultCategory = "Default";

        [SerializeField]
        private int poolSize = 8;

        private AudioSource[] _pool;
        private readonly Dictionary<string, float> _categoryVolumes = new Dictionary<string, float>();

        protected virtual void Awake()
        {
            _pool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _pool[i] = source;
            }
        }

        public void PlaySFX(AudioClip clip, float volume = 1f) =>
            PlaySFX(clip, DefaultCategory, volume);

        /// <summary>Category-aware overload. Not part of IAudioManager — callers that care
        /// which SetCategoryVolume bucket a clip draws from use this directly.</summary>
        public void PlaySFX(AudioClip clip, string category, float volume = 1f)
        {
            if (clip == null)
                return;

            AudioSource source = GetFreeSource();
            if (source == null)
                return; // pool exhausted — drop the sound, see class doc comment

            float categoryMultiplier = _categoryVolumes.TryGetValue(category, out float m) ? m : 1f;
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume * categoryMultiplier);
            source.Play();
        }

        public void SetCategoryVolume(string category, float volume)
        {
            _categoryVolumes[category] = volume;
        }

        private AudioSource GetFreeSource()
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                if (!_pool[i].isPlaying)
                    return _pool[i];
            }

            return null;
        }
    }
}
