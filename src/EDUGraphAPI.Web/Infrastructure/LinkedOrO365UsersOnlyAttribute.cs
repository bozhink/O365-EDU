/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Infrastructure
{
    using System.Web;
    using System.Web.Mvc;
    using EDUGraphAPI.Web.Services;

    /// <summary>
    /// Only allow linked users or Office 365 users to visit the protected controllers/actions
    /// </summary>
    public class LinkedOrO365UsersOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var applicationService = DependencyResolver.Current.GetService<IApplicationService>();
            var user = applicationService.GetUserContext();
            return user.AreAccountsLinked || user.IsO365Account;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new ViewResult
            {
                ViewName = "NoAccess"
            };
        }
    }
}
