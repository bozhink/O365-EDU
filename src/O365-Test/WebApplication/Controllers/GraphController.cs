using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Resources;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class GraphController : Controller
    {
        private readonly IGraphService service;
        private readonly IAuthProvider authProvider;

        public GraphController(IGraphService service, IAuthProvider authProvider)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (authProvider == null)
            {
                throw new ArgumentNullException(nameof(authProvider));
            }

            this.service = service;
            this.authProvider = authProvider;
        }

        // GET: Graph
        public ActionResult Index()
        {
            return this.View();
        }

        [Authorize]
        // Get the current user's email address from their profile.
        public async Task<ActionResult> GetMyEmailAddress()
        {
            try
            {
                // Get an access token.
                string accessToken = await this.authProvider.GetUserAccessTokenAsync();

                // Get the current user's email address.
                this.ViewBag.Email = await this.service.GetMyEmailAddress(accessToken);
                return this.View("Index");
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }

        [Authorize]
        // Send mail on behalf of the current user.
        public async Task<ActionResult> SendEmail()
        {
            if (string.IsNullOrEmpty(this.Request.Form["email-address"]))
            {
                this.ViewBag.Message = Resource.Graph_SendMail_Message_GetEmailFirst;
                return this.View("Index");
            }

            // Build the email message.
            MessageRequest email = this.service.BuildEmailMessage(this.Request.Form["recipients"], this.Request.Form["subject"]);
            try
            {
                // Get an access token.
                string accessToken = await this.authProvider.GetUserAccessTokenAsync();

                // Send the email.
                this.ViewBag.Message = await this.service.SendEmail(accessToken, email);

                // Reset the current user's email address and the status to display when the page reloads.
                this.ViewBag.Email = this.Request.Form["email-address"];
                return this.View("Index");
            }
            catch (Exception e)
            {
                if (e.Message == Resource.Error_AuthChallengeNeeded)
                {
                    return new EmptyResult();
                }

                return this.RedirectToAction("Index", "Error", new { message = Resource.Error_Message + Request.RawUrl + ": " + e.Message });
            }
        }
    }
}
