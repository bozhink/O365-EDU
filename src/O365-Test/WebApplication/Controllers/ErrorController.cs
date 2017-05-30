using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index(string message)
        {
            this.ViewBag.Message = message;
            return this.View("Error");
        }
    }
}
