/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Infrastructure
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.OpenIdConnect;

    /// <summary>
    /// Handle AdalException and navigate user to the authorize endpoint or /Link/LoginO365Required
    /// </summary>
    public class HandleAdalExceptionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public const string ChallengeImmediatelyTempDataKey = "ChallengeImmediately";

        public void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.Exception is AdalException))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        error = "AdalException"
                    }
                };

                filterContext.ExceptionHandled = true;
                return;
            }

            var requestUrl = filterContext.HttpContext.Request.Url.ToString();
            var challengeImmediately = filterContext.Controller.TempData[ChallengeImmediatelyTempDataKey];
            if (challengeImmediately as bool? == true)
            {
                var properties = new AuthenticationProperties
                {
                    RedirectUri = requestUrl
                };

                filterContext.HttpContext.GetOwinContext().Authentication.Challenge(properties, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                var redirectTo = "/Link/LoginO365Required?returnUrl=" + Uri.EscapeDataString(requestUrl);
                filterContext.Result = new RedirectResult(redirectTo);
            }

            filterContext.ExceptionHandled = true;
        }
    }
}
