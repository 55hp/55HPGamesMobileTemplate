using System;

namespace hp55games.Mobile.Core.Narrative
{
    /// <summary>
    /// Contract for a single cutscene's playback. No ServiceRegistry registration implied —
    /// a cutscene is typically per-scene/per-trigger, not a singleton global service. Pure
    /// interface, no base class: a cutscene might be Timeline-driven, Animator-driven, or
    /// fully scripted, too divergent to share a MonoBehaviour skeleton.
    /// </summary>
    public interface ICutsceneController
    {
        void PlayCutscene();

        void SkipCutscene();

        bool IsPlaying { get; }

        event Action OnCutsceneFinished;
    }
}
