using UnityEngine;

namespace hp55games.Mobile.Core.UI
{
    /// <summary>
    /// Simple runtime helper to know when all UI services
    /// (navigation, popups, music, etc.) are fully registered.
    /// </summary>
    public static class UIRuntime
    {
        /// <summary>
        /// True when UIServiceInstaller has completed registration.
        /// </summary>
        public static bool ServicesReady { get; private set; }

        /// <summary>
        /// Call this once, at the end of UIServiceInstaller.Awake().
        /// </summary>
        public static void MarkServicesReady()
        {
            if (ServicesReady)
                return;

            ServicesReady = true;
            Debug.Log("[UIRuntime] UI services marked as READY.");
        }
    }
}