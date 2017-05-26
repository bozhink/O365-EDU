/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.SyncData
{
    using System.IO;
    using System.Threading.Tasks;
    using EDUGraphAPI.Constants;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Services.DataSync;
    using EDUGraphAPI.Services.DifferentialQuery;
    using EDUGraphAPI.Utils;
    using Microsoft.Azure.WebJobs;

    public class Functions
    {
        public static async Task SyncUsersAsync([TimerTrigger("0 * * * * *")] TimerInfo timerInfo, TextWriter log)
        {
            var db = new ApplicationDbContext("SyncDataWebJobDefaultConnection");
            var userSyncService = new UserSyncService(db, GetTenantAccessTokenAsync, new DifferentialQueryServiceFactory(), log);
            await userSyncService.SyncAsync();
        }

        private static Task<string> GetTenantAccessTokenAsync(string tenantId)
        {
            return AuthenticationHelper.GetAppOnlyAccessTokenForDaemonAppAsync(
                Resources.AADGraph,
                ConfigurationConstants.CertPath,
                ConfigurationConstants.CertPassword,
                tenantId);
        }
    }
}
