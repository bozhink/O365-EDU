using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using WebApplication.Constants;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IAuthProvider authProvider;

        public UserProfileController(IAuthProvider authProvider)
        {
            if (authProvider == null)
            {
                throw new ArgumentNullException(nameof(authProvider));
            }

            this.authProvider = authProvider;
        }

        // GET: UserProfile
        public async Task<ActionResult> Index()
        {
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            try
            {
                Uri servicePointUri = new Uri(ResourceConstants.GraphResourceID);
                Uri serviceRoot = new Uri(servicePointUri, tenantID);
                var activeDirectoryClient = new ActiveDirectoryClient(
                    serviceRoot,
                    async () => await this.authProvider.GetUserAccessTokenAsync());

                // use the token for querying the graph to get the user details

                var result = await activeDirectoryClient.Users
                    .Where(u => u.ObjectId.Equals(userObjectID))
                    .ExecuteAsync();

                IUser user = result.CurrentPage.ToList().First();

                return this.View(user);
            }
            catch (AdalException)
            {
                // Return to error page.
                return this.View("Error");
            }
            // if the above failed, the user needs to explicitly re-authenticate for the app to obtain the required token
            catch (Exception)
            {
                return this.View("Relogin");
            }
        }

        public void RefreshSession()
        {
            HttpContext.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = "/UserProfile"
                },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}
