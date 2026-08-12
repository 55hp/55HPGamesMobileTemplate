using UnityEngine;
using hp55games.Mobile.Core.Architecture;
using hp55games.Mobile.Core.UI;

namespace hp55games.Mobile.Core.Haptics
{
    /// <summary>
    /// Default cross-platform IHapticsService. Unity's built-in API (Handheld.Vibrate)
    /// only exposes a single generic device vibration, not graded light/medium/heavy
    /// impacts — real per-intensity haptics requires a native plugin. All four methods
    /// currently trigger the same vibration; the graded interface is kept so call sites
    /// don't need to change when a native haptics plugin is added later.
    ///
    /// Respects IUIOptionsService.Haptics: does nothing if the player disabled haptics
    /// in Options. Handheld.Vibrate() itself is a no-op outside Android/iOS, so this is
    /// safe to call unconditionally from any platform, including the Editor.
    /// </summary>
    public sealed class HapticsService : IHapticsService
    {
        public void Selection() => Vibrate();
        public void Light() => Vibrate();
        public void Medium() => Vibrate();
        public void Heavy() => Vibrate();

        private static void Vibrate()
        {
            if (ServiceRegistry.TryResolve<IUIOptionsService>(out var options) && !options.Haptics)
                return;

            Handheld.Vibrate();
        }
    }
}
