using System.Threading.Tasks;

namespace hp55games.Mobile.Core.Save
{
    /// <summary>
    /// Identity prerequisite for ICloudSaveService — UGS Cloud Save requires a signed-in
    /// player before any sync call works, so this exists to be resolved and awaited before
    /// ICloudSaveService is used, not as an optional add-on.
    ///
    /// Named to match this template's own service-interface convention, not to be confused
    /// with Unity.Services.Authentication.IAuthenticationService (the UGS SDK's own interface
    /// of the same short name, different namespace) — UGSAuthenticationService wraps that SDK
    /// type internally but this interface is what gameplay/UI code depends on.
    ///
    /// Not registered by ServiceRegistry.InstallDefaults() and may legitimately be absent at
    /// runtime — UnityServices.InitializeAsync() can fail (no network, UGS project not linked
    /// for this Unity project). Resolve with
    /// ServiceRegistry.TryResolve&lt;IAuthenticationService&gt;(), not Resolve&lt;T&gt;(),
    /// since unlike every other default service it may not be there.
    /// </summary>
    public interface IAuthenticationService
    {
        Task<bool> SignInAnonymouslyAsync();

        bool IsSignedIn { get; }

        string PlayerId { get; }
    }
}
