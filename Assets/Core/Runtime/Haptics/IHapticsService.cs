namespace hp55games.Mobile.Core.Haptics
{
    /// <summary>
    /// Device vibration feedback for taps, impacts and selection changes. Graded by
    /// intensity so call sites (button taps vs. collisions vs. big wins) don't need to
    /// change if a native haptics plugin is added later — see HapticsService for the
    /// current default implementation's limits.
    /// </summary>
    public interface IHapticsService
    {
        /// <summary>Subtle feedback: UI selection changes (toggles, sliders, list scroll snaps).</summary>
        void Selection();

        /// <summary>Light impact: regular button taps, small pickups.</summary>
        void Light();

        /// <summary>Medium impact: confirmations, scoring, moderate hits.</summary>
        void Medium();

        /// <summary>Heavy impact: big wins, deaths, major collisions.</summary>
        void Heavy();
    }
}
