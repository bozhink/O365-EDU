/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Models
{
    using EDUGraphAPI.Data.Models;

    public class AdminContext
    {
        public AdminContext(Organization organization)
        {
            this.Organization = organization;
        }

        public Organization Organization { get; private set; }

        public bool IsAdminConsented => this.Organization != null && this.Organization.IsAdminConsented;
    }
}
