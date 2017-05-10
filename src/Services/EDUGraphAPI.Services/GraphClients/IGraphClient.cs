/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.GraphClients
{
    using System.Threading.Tasks;
    using EDUGraphAPI.Services.Models.GraphClients;

    public interface IGraphClient
    {
        Task<UserInfo> GetCurrentUserAsync();

        Task<TenantInfo> GetTenantAsync(string tenantId);
    }
}
