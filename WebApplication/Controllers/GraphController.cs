using System;
using System.Web.Mvc;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    public class GraphController : Controller
    {
        private readonly IGraphService service;

        public GraphController(IGraphService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            this.service = service;
        }

        // GET: Graph
        public ActionResult Index()
        {
            return this.View();
        }
    }
}
