﻿/*
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
    using Microsoft.Azure.ActiveDirectory.GraphClient;

    public class AADGraphClient : IGraphClient
    {
        private readonly ActiveDirectoryClient activeDirectoryClient;

        public AADGraphClient(ActiveDirectoryClient activeDirectoryClient)
        {
            if (activeDirectoryClient == null)
            {
                throw new ArgumentNullException(nameof(activeDirectoryClient));
            }

            this.activeDirectoryClient = activeDirectoryClient;
        }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            var me = await this.activeDirectoryClient.Me.ExecuteAsync();
            return new UserInfo
            {
                Id = me.ObjectId,
                GivenName = me.GivenName,
                Surname = me.Surname,
                Mail = me.Mail,
                UserPrincipalName = me.UserPrincipalName,
                Roles = await this.GetRolesAsync(me)
            };
        }

        public async Task<TenantInfo> GetTenantAsync(string tenantId)
        {
            var tenant = await this.activeDirectoryClient.TenantDetails
                .Where(i => i.ObjectId == tenantId)
                .ExecuteSingleAsync();

            return new TenantInfo
            {
                Id = tenant.ObjectId,
                Name = tenant.DisplayName
            };
        }

        private async Task<string[]> GetRolesAsync(IUser user)
        {
            var roles = new List<string>();
            var directoryAdminRole = await this.GetDirectoryAdminRoleAsync();

            if (await directoryAdminRole.Members.AnyAsync(i => i.ObjectId == user.ObjectId))
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

        private async Task<IDirectoryRole> GetDirectoryAdminRoleAsync()
        {
            var roles = await this.activeDirectoryClient.DirectoryRoles
               .Expand(i => i.Members)
               .ExecuteAllAsync();

            return roles.FirstOrDefault(i => i.DisplayName == EDUGraphAPI.Constants.Common.AADCompanyAdminRoleName);
        }
    }
}
