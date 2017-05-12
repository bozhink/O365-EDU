/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Infrastructure
{
    using System.Web.Mvc;
    using EDUGraphAPI.Services.Web;

    /// <summary>
    /// Allow the web app to redirect the current user to the proper login page in our multi-authentication-method scenario
    /// </summary>
    public class EduAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        error = "Unauthorized"
                    }
                };
            }
            else
            {
                CookieService cookieService = new CookieService();
                string username = cookieService.GetCookiesOfUsername();
                string email = cookieService.GetCookiesOfEmail();
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email))
                {
                    filterContext.Result = new RedirectResult("/Account/O365login");
                }
                else
                {
                    filterContext.Result = new RedirectResult("/Account/Login");
                }
            }
        }
    }
}
