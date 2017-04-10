using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace WebApplication.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!this.Request.IsAuthenticated)
            {
                this.HttpContext.GetOwinContext()
                    .Authentication
                    .Challenge(
                        new AuthenticationProperties
                        {
                            RedirectUri = "/"
                        },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            this.HttpContext.GetOwinContext()
                .Authentication
                .SignOut(
                    new AuthenticationProperties
                    {
                        RedirectUri = callbackUrl
                    },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (this.Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }
    }
}
