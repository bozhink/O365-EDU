/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// An instance of the class represents a user
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? OrganizationId { get; set; }

        [ForeignKey(nameof(ApplicationUser.OrganizationId))]
        public virtual Organization Organization { get; set; }

        public string O365UserId { get; set; }

        public string O365Email { get; set; }

        public string JobTitle { get; set; }

        public string Department { get; set; }

        public string Mobile { get; set; }

        public string FavoriteColor { get; set; }

        public string FullName => $"{this.FirstName} {this.LastName}";
    }
}
