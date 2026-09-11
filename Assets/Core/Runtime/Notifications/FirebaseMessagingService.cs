#if FIREBASE_MESSAGING_ENABLED
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Messaging;

namespace hp55games.Mobile.Core.Notifications
{
    /// <summary>
    /// IPushNotificationService backed by Firebase.Messaging.FirebaseMessaging.
    ///
    /// Compiled ONLY when FIREBASE_MESSAGING_ENABLED is defined — same reasoning as
    /// FirebaseAnalyticsService/FirebaseCrashReportingService (Assets/Core/Runtime/Analytics/,
    /// dossier Fase 2): this template ships without the Firebase Unity SDK, so this file must
    /// not be part of the default Core.Runtime compile set until a concrete project has
    /// actually imported Firebase.Messaging and defined the symbol itself. See those two
    /// files' doc comments for the full rationale — not restated here. Not registered by
    /// ServiceRegistry.InstallDefaults(), same per-project-config reasoning as every other
    /// Firebase adapter.
    /// </summary>
    public sealed class FirebaseMessagingService : IPushNotificationService
    {
        public event Action<string> OnTokenRefreshed;
        public event Action<Dictionary<string, string>> OnMessageReceived;

        public FirebaseMessagingService()
        {
            FirebaseMessaging.TokenReceived += HandleTokenReceived;
            FirebaseMessaging.MessageReceived += HandleMessageReceived;
        }

        public Task<string> GetTokenAsync() => FirebaseMessaging.GetTokenAsync();

        private void HandleTokenReceived(object sender, TokenReceivedEventArgs e)
        {
            OnTokenRefreshed?.Invoke(e.Token);
        }

        private void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived?.Invoke(new Dictionary<string, string>(e.Message.Data));
        }
    }
}
#endif
