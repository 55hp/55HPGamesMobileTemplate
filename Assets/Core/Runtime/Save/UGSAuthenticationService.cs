using System.Threading.Tasks;
using UGSAuth = Unity.Services.Authentication;

namespace hp55games.Mobile.Core.Save
{
    /// <summary>
    /// IAuthenticationService backed by Unity.Services.Authentication.AuthenticationService.
    ///
    /// Imported via an alias (UGSAuth), not a plain "using Unity.Services.Authentication;" —
    /// that namespace declares its own Unity.Services.Authentication.IAuthenticationService,
    /// identically named to this template's IAuthenticationService. A plain using directive
    /// would make every unqualified "IAuthenticationService" reference in this file ambiguous
    /// (CS0104) between the two. Same fix shape as the Phase 3 Camera namespace collision:
    /// avoid the collision by not importing the colliding name unqualified, rather than
    /// renaming this template's interface.
    ///
    /// Constructed and registered only after UnityServices.InitializeAsync() has completed
    /// successfully — see GameBootstrap.BootstrapSequence() — never registered synchronously
    /// inside ServiceRegistry.InstallDefaults(), since the underlying SDK isn't initialized
    /// yet at that point in the bootstrap sequence.
    /// </summary>
    public sealed class UGSAuthenticationService : IAuthenticationService
    {
        public bool IsSignedIn => UGSAuth.AuthenticationService.Instance.IsSignedIn;

        public string PlayerId => UGSAuth.AuthenticationService.Instance.PlayerId;

        public async Task<bool> SignInAnonymouslyAsync()
        {
            if (UGSAuth.AuthenticationService.Instance.IsSignedIn)
                return true;

            try
            {
                await UGSAuth.AuthenticationService.Instance.SignInAnonymouslyAsync();
                return true;
            }
            catch (UGSAuth.AuthenticationException)
            {
                return false;
            }
            catch (Unity.Services.Core.RequestFailedException)
            {
                return false;
            }
        }
    }
}
