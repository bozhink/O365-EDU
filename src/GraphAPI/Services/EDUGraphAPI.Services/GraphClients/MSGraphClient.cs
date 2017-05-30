/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.GraphClients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EDUGraphAPI.Services.Models.GraphClients;
    using Microsoft.Graph;

    public class MSGraphClient : IGraphClient
    {
        private readonly GraphServiceClient graphServiceClient;

        public MSGraphClient(GraphServiceClient graphServiceClient)
        {
            if (graphServiceClient == null)
            {
                throw new ArgumentNullException(nameof(graphServiceClient));
            }

            this.graphServiceClient = graphServiceClient;
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            var me = await this.graphServiceClient.Me.Request()
                .Select("id,givenName,surname,userPrincipalName,assignedLicenses")
                .GetAsync();

            return new UserInfo
            {
                Id = me.Id,
                GivenName = me.GivenName,
                Surname = me.Surname,
                Mail = me.Mail,
                UserPrincipalName = me.UserPrincipalName,
                Roles = await this.GetRolesAsync(me)
            };
        }

        public async Task<TenantInfo> GetTenantAsync(string tenantId)
        {
            var tenant = await this.graphServiceClient.Organization[tenantId].Request().GetAsync();
            return new TenantInfo
            {
                Id = tenant.Id,
                Name = tenant.DisplayName
            };
        }

        public async Task<string[]> GetRolesAsync(User user)
        {
            var roles = new List<string>();
            var directoryAdminRole = await this.GetDirectoryAdminRoleAsync();
            if (await directoryAdminRole.Members.AnyAsync(i => i.Id == user.Id))
            {
                roles.Add(EDUGraphAPI.Constants.Roles.Admin);
            }

            if (user.AssignedLicenses.Any(i => i.SkuId == EDUGraphAPI.Constants.O365ProductLicenses.Faculty || i.SkuId == EDUGraphAPI.Constants.O365ProductLicenses.FacultyPro))
            {
                roles.Add(EDUGraphAPI.Constants.Roles.Faculty);
            }

            if (user.AssignedLicenses.Any(i => i.SkuId == EDUGraphAPI.Constants.O365ProductLicenses.Student || i.SkuId == EDUGraphAPI.Constants.O365ProductLicenses.StudentPro))
            {
                roles.Add(EDUGraphAPI.Constants.Roles.Student);
            }

            return roles.ToArray();
        }

        private async Task<DirectoryRole> GetDirectoryAdminRoleAsync()
        {
            var roles = await this.graphServiceClient.DirectoryRoles.Request()
                .Expand(i => i.Members)
                .GetAllAsync();

            return roles.FirstOrDefault(i => i.DisplayName == EDUGraphAPI.Constants.Common.AADCompanyAdminRoleName);
        }
    }
}
