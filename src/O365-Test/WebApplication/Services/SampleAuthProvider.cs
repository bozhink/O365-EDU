using System;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Resources;
using WebApplication.Models;
using WebApplication.Constants;

namespace WebApplication.Services
{
    public class SampleAuthProvider : IAuthProvider
    {
        private readonly string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private readonly string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private readonly string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private readonly string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private readonly string redirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private readonly string graphScopes = ConfigurationManager.AppSettings["ida:GraphScopes"];

        public async Task<string> GetUserAccessTokenAsync()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            var clientcred = new ClientCredential(clientId, clientSecret);

            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            var authenticationContext = new AuthenticationContext(aadInstance + tenantID, new ADALTokenCache(signedInUserID));

            try
            {
                var authenticationResult = await authenticationContext.AcquireTokenSilentAsync(
                    ResourceConstants.GraphResourceID,
                    clientcred,
                    new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));

                return authenticationResult.AccessToken;
            }
            catch (Exception e)
            {
                HttpContext.Current.Request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties
                    {
                        RedirectUri = "/"
                    },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);

                throw new Exception(Resource.Error_AuthChallengeNeeded, e);
            }
        }
    }
}
