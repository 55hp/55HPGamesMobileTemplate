using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Notifications
{
    /// <summary>
    /// SDK-agnostic push notification contract, like every other service interface in this
    /// template — gameplay/server-integration code depends only on this, never on Firebase
    /// directly. GetTokenAsync's result is the device/registration token a server needs to
    /// target this specific device; OnTokenRefreshed fires when that token changes (token
    /// rotation is normal, not an error condition); OnMessageReceived delivers the raw
    /// key/value payload of a foreground message, left as a Dictionary rather than a typed
    /// payload since message schemas are entirely project-defined.
    /// </summary>
    public interface IPushNotificationService
    {
        Task<string> GetTokenAsync();

        event Action<string> OnTokenRefreshed;

        event Action<Dictionary<string, string>> OnMessageReceived;
    }
}
