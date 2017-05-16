/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Controllers
{
    using System;
    using System.Web.Mvc;
    using EDUGraphAPI.Constants;
    using EDUGraphAPI.Services.Web;

    public class DemoHelperController : Controller
    {
        private readonly IDemoHelperService service;

        public DemoHelperController(IDemoHelperService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            this.service = service;
        }

        public ActionResult Control()
        {
            var parentActionRouteData = this.ControllerContext.ParentActionViewContext.RouteData;
            var page = this.service.GetDemoPage(
                parentActionRouteData.GetRequiredString(ContextKeys.Controller),
                parentActionRouteData.GetRequiredString(ContextKeys.Action));

            return this.PartialView(page);
        }
    }
}
