/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Models
{
    using System.Web;
    using EDUGraphAPI.Data.Models;
    using Microsoft.AspNet.Identity;

    /// <summary>
    /// Context for the logged-in user
    /// </summary>
    public class UserContext
    {
        public UserContext(HttpContext httpContext, ApplicationUser user)
        {
            this.HttpContext = httpContext;
            this.User = user;
        }

        /// <summary>
        /// The HttpContext
        /// </summary>
        public HttpContext HttpContext { get; private set; }

        /// <summary>
        /// The underlying user
        /// </summary>
        /// For unlinked Office 365 user, the property is null.
        /// <remarks>
        public ApplicationUser User { get; private set; }

        /// <summary>
        /// Display name of the logged-in user
        /// </summary>
        public string UserDisplayName => this.HttpContext.User.Identity.GetUserName();

        /// <summary>
        /// Is the logged-in account a local account
        /// </summary>
        public bool IsLocalAccount => this.User != null && this.User.Id == this.HttpContext.User.Identity.GetUserId();

        /// <summary>
        /// Is the logged-in account an Office 365 account
        /// </summary>
        public bool IsO365Account => !this.IsLocalAccount;

        /// <summary>
        /// Are the local account and Office 365 account linked
        /// </summary>
        public bool AreAccountsLinked => this.User != null && this.User.O365UserId.IsNotNullAndEmpty();

        /// <summary>
        /// Is the logged-in user a faculty (teacher)
        /// </summary>
        public bool IsFaculty => this.HttpContext.User.IsInRole(EDUGraphAPI.Constants.Roles.Faculty);

        /// <summary>
        /// Is the logged-in user a student
        /// </summary>
        public bool IsStudent => this.HttpContext.User.IsInRole(EDUGraphAPI.Constants.Roles.Student);

        /// <summary>
        /// Is the logged-in user an administrator
        /// </summary>
        public bool IsAdmin => this.HttpContext.User.IsInRole(EDUGraphAPI.Constants.Roles.Admin);

        /// <summary>
        /// Is the use's tenant consented by a admin
        /// </summary>
        public bool IsTenantConsented => this.User != null && this.User.Organization != null && this.User.Organization.IsAdminConsented;

        /// <summary>
        /// Get the email of the Office 365 account
        /// </summary>
        /// <remarks>
        /// For unlinked local account, the value is null
        /// </remarks>
        public string UserO365Email => this.AreAccountsLinked ? this.User.O365Email : (this.IsO365Account ? this.HttpContext.User.Identity.Name : null);
    }
}
