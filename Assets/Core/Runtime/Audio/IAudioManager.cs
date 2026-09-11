using UnityEngine;

namespace hp55games.Mobile.Core.Audio
{
    /// <summary>
    /// One-shot SFX playback. Deliberately synchronous and AudioClip-based — NOT Task-based
    /// Addressables-loaded like UI.IMusicService, and this is not an oversight to "fix" into
    /// consistency with it: a footstep/hit/UI-click sound must fire the instant PlaySFX is
    /// called, with zero loading latency, so callers hand over an already-loaded AudioClip
    /// reference. A BGM track has seconds of runway before it's audible, which is exactly what
    /// makes IMusicService's async Addressables-address loading + crossfade affordable there
    /// and unacceptable here. Same "service" shape, different job — don't unify them.
    /// </summary>
    public interface IAudioManager
    {
        void PlaySFX(AudioClip clip, float volume = 1f);

        void SetCategoryVolume(string category, float volume);
    }
}
