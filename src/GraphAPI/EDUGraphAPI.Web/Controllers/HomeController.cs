/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using EDUGraphAPI.Web.Infrastructure;
    using EDUGraphAPI.Web.Services;

    [EduAuthorize]
    public class HomeController : Controller
    {
        private IApplicationService applicationService;

        public HomeController(IApplicationService applicationService)
        {
            if (applicationService == null)
            {
                throw new ArgumentNullException(nameof(applicationService));
            }

            this.applicationService = applicationService;
        }

        //
        // GET: /Home/Index
        public async Task<ActionResult> Index()
        {
            var context = await applicationService.GetUserContextAsync();
            if (context.AreAccountsLinked)
            {
                if (context.IsAdmin && !context.IsTenantConsented)
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Schools");
            }
            else
            {
                return RedirectToAction("Index", "Link");
            }
        }

        //
        // GET: /Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //
        // GET: /Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
