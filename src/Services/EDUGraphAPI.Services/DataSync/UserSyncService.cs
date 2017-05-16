/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.DataSync
{
    using System;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Data.Models;
    using EDUGraphAPI.Infrastructure;
    using EDUGraphAPI.Services.DifferentialQuery;
    using EDUGraphAPI.Services.Models.DataSync;
    using EDUGraphAPI.Services.Models.DifferentialQuery;

    public delegate Task<string> GetTenantAccessTokenAsyncDelegate(string tenantId);

    /// <summary>
    /// An instance of the class handles syncing users in local database with differential query.
    /// </summary>
    public class UserSyncService : IUserSyncService
    {
        private const string UsersQuery = "users";
        private const string APIVersion = "1.5";

        private readonly TextWriter log;
        private readonly ApplicationDbContext db;
        private readonly GetTenantAccessTokenAsyncDelegate getTenantAccessTokenAsync;
        private readonly IDifferentialQueryServiceFactory differentialQueryServiceFactory;

        public UserSyncService(ApplicationDbContext db, GetTenantAccessTokenAsyncDelegate getTenantAccessTokenAsync, IDifferentialQueryServiceFactory differentialQueryServiceFactory, TextWriter log)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            if (getTenantAccessTokenAsync == null)
            {
                throw new ArgumentNullException(nameof(getTenantAccessTokenAsync));
            }

            if (differentialQueryServiceFactory == null)
            {
                throw new ArgumentNullException(nameof(differentialQueryServiceFactory));
            }

            this.db = db;
            this.getTenantAccessTokenAsync = getTenantAccessTokenAsync;
            this.differentialQueryServiceFactory = differentialQueryServiceFactory;
            this.log = log;
        }

        public async Task SyncAsync()
        {
            try
            {
                await this.SyncAsyncCore();
            }
            catch (Exception ex)
            {
                await this.WriteLogAsync("Failed to sync users. Error: " + ex.Message);
            }
        }

        private async Task SyncAsyncCore()
        {
            var consentedOrganizations = await this.db.Organizations
                .Where(i => i.IsAdminConsented)
                .ToArrayAsync();

            if (!consentedOrganizations.Any())
            {
                await this.WriteLogAsync($"No consented organization found. This sync was canceled.");
                return;
            }

            foreach (var org in consentedOrganizations)
            {
                try
                {
                    await this.SyncOrganizationUsersAsync(org);
                    this.db.SaveChanges();
                    await this.WriteLogAsync($"All the changes were saved.");
                }
                catch (Exception ex)
                {
                    await this.WriteLogAsync($"Failed to sync users of {org.Name}. Error: {ex.Message}");
                }
            }
        }

        private async Task SyncOrganizationUsersAsync(Organization org)
        {
            await this.WriteLogAsync($"Starting to sync users for the {org.Name} organization.");

            var dataSyncRecord = await this.GetOrCreateDataSyncRecord(org.TenantId, UsersQuery);

            await this.WriteLogAsync($"Send Differential Query.");
            if (dataSyncRecord.Id == 0)
            {
                await this.WriteLogAsync("First time executing differential query; all items will return.");
            }

            var differentialQueryService = new DifferentialQueryService(() => this.getTenantAccessTokenAsync.Invoke(org.TenantId));

            var result = await differentialQueryService.QueryAsync<User>(dataSyncRecord.DeltaLink, APIVersion);
            await this.WriteLogAsync($"Get {result.Items.Length} users.");

            foreach (var differentialUser in result.Items)
            {
                await this.UpdateUserAsync(differentialUser);
            }

            dataSyncRecord.DeltaLink = result.DeltaLink;
            dataSyncRecord.Updated = DateTime.UtcNow;
        }

        private async Task<DataSyncRecord> GetOrCreateDataSyncRecord(string tenantId, string query)
        {
            var record = await this.db.DataSyncRecords
                .Where(i => i.TenantId == tenantId)
                .Where(i => i.Query == query)
                .FirstOrDefaultAsync();

            if (record == null)
            {
                var url = string.Format("{0}/{1}/{2}?deltaLink=", EDUGraphAPI.Constants.Resources.AADGraph, tenantId, query);
                record = new DataSyncRecord
                {
                    Query = query,
                    TenantId = tenantId,
                    DeltaLink = url
                };

                this.db.DataSyncRecords.Add(record);
            }

            return record;
        }

        private async Task UpdateUserAsync(Delta<User> differentialUser)
        {
            var user = await this.db.Users
                .Where(i => i.O365UserId == differentialUser.Entity.ObjectId)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                await this.WriteLogAsync("Skipping updating user {0} who does not exist in the local database.", differentialUser.Entity.ObjectId);
                return;
            }

            if (!differentialUser.IsDeleted)
            {
                if (differentialUser.ModifiedPropertyNames.Any())
                {
                    SimpleMapper.Map(differentialUser.Entity, user, differentialUser.ModifiedPropertyNames);
                    await this.WriteLogAsync("Updated user {0}. Changed properties: {1}", user.O365Email, string.Join(", ", differentialUser.ModifiedPropertyNames));
                }
                else
                {
                    await this.WriteLogAsync("Skipped updating user {0}, because the properties that changed are not included in the local database.", differentialUser.Entity.ObjectId);
                }
            }
            else
            {
                this.db.Users.Remove(user);
                await this.WriteLogAsync($"Deleted user {user.Email}.");
                return;
            }
        }

        private async Task WriteLogAsync(string message)
        {
            await this.log.WriteAsync($"[{DateTime.UtcNow}][SyncData] ");
            await this.log.WriteLineAsync(message);
        }

        private async Task WriteLogAsync(string message, params object[] args)
        {
            await this.WriteLogAsync(string.Format(message, args));
        }
    }
}
