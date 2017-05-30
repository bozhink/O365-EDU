﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using System.Web;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Data.Models;
    using EDUGraphAPI.Services.Models.GraphClients;
    using EDUGraphAPI.Web.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    /// <summary>
    /// An instance of the class handles getting/updating user/organization
    /// </summary>
    public class ApplicationService : IApplicationService
    {
        private static readonly string UserContextKey = typeof(UserContext).Name + "Context";
        private static readonly string AdminContextKey = typeof(UserContext).Name + "AdminContextKey";

        private HttpContextBase httpContext;
        private ApplicationDbContext dbContext;
        private ApplicationUserManager userManager;

        public ApplicationService(ApplicationDbContext dbContext, ApplicationUserManager userManager, HttpContextBase httpContext)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.httpContext = httpContext;
        }

        public ApplicationService(ApplicationDbContext dbContext, ApplicationUserManager userManager)
            : this(dbContext, userManager, new HttpContextWrapper(HttpContext.Current))
        {
        }

        /// <summary>
        /// Get current user
        /// </summary>
        public ApplicationUser GetCurrentUser()
        {
            var userId = this.GetUserId();
            if (userId.IsNullOrEmpty())
            {
                return null;
            }

            return dbContext.Users
                .Include(i => i.Organization)
                .FirstOrDefault(i => i.Id == userId || i.O365UserId == userId);
        }

        /// <summary>
        /// Get current user
        /// </summary>
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = this.GetUserId();
            if (userId.IsNullOrEmpty())
            {
                return null;
            }

            return await dbContext.Users
                .Include(i => i.Organization)
                .FirstOrDefaultAsync(i => i.Id == userId || i.O365UserId == userId);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        public async Task<ApplicationUser> GetUserAsync(string id)
        {
            return await dbContext.Users
                .Include(i => i.Organization)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Get user by email.
        /// </summary>
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await dbContext.Users
                .Include(i => i.Organization)
                .FirstOrDefaultAsync(i => i.Email == email);
        }

        /// <summary>
        /// Update current user's favorite color
        /// </summary>
        public void UpdateUserFavoriteColor(string color)
        {
            var user = GetCurrentUser();
            if (user != null)
            {
                user.FavoriteColor = color;
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Get current user's context
        /// </summary>
        public UserContext GetUserContext()
        {
            var currentUser = GetCurrentUser();
            return new UserContext(HttpContext.Current, currentUser);
        }

        /// <summary>
        /// Get current user's context
        /// </summary>
        public async Task<UserContext> GetUserContextAsync()
        {
            var currentUser = await GetCurrentUserAsync();
            return new UserContext(HttpContext.Current, currentUser);
        }

        /// <summary>
        /// Get current admin's context
        /// </summary>
        public async Task<AdminContext> GetAdminContextAsync()
        {
            var currentOrganization = await GetCurrentTenantAsync();
            return new AdminContext(currentOrganization);
        }

        /// <summary>
        /// Is the specified O365 account linked with an local account
        /// </summary>
        /// <param name="o365UserId"></param>
        /// <returns></returns>
        public Task<bool> IsO365AccountLinkedAsync(string o365UserId)
        {
            return dbContext.Users.AnyAsync(i => i.O365UserId == o365UserId);
        }

        /// <summary>
        /// Update the local user with O365 user and tenant info
        /// </summary>
        public async Task UpdateLocalUserAsync(ApplicationUser localUser, UserInfo o365User, TenantInfo tenant)
        {
            await UpdateUserNoSaveAsync(localUser, o365User, tenant);
            await dbContext.SaveChangesAsync();

            await UpdateUserRoles(localUser, o365User);
        }

        /// <summary>
        /// Create or update the organization
        /// </summary>
        public async Task CreateOrUpdateOrganizationAsync(TenantInfo tenant, bool adminConsented)
        {
            var organization = await dbContext.Organizations
                .FirstOrDefaultAsync(i => i.TenantId == tenant.Id);

            if (organization == null)
            {
                organization = new Organization();
                dbContext.Organizations.Add(organization);
            }

            InitOrganization(organization, tenant);

            organization.IsAdminConsented = adminConsented;
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Update organization
        /// </summary>
        public async Task UpdateOrganizationAsync(string tenantId, bool adminConsented)
        {
            var organization = await dbContext.Organizations
                  .FirstOrDefaultAsync(i => i.TenantId == tenantId);

            if (organization == null)
            {
                return;
            }

            organization.IsAdminConsented = adminConsented;
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Get linked users with the specified filter
        /// </summary>
        public async Task<ApplicationUser[]> GetLinkedUsers(Expression<Func<ApplicationUser, bool>> predicate = null)
        {
            return await dbContext.Users
                .Where(i => !string.IsNullOrEmpty(i.O365UserId))
                .WhereIf(predicate, predicate != null)
                .ToArrayAsync();
        }

        /// <summary>
        /// Unlink the specified the account
        /// </summary>
        /// <param name="id">User id</param>
        public async Task UnlinkAccountsAsync(string id)
        {
            var user = await GetUserAsync(id);

            // Remove token caches
            var caches = await dbContext.UserTokenCacheList
                .Where(i => i.WebUserUniqueId == user.O365UserId)
                .ToArrayAsync();

            dbContext.UserTokenCacheList.RemoveRange(caches);

            // remove o365 user info
            user.O365Email = null;
            user.O365UserId = null;
            await dbContext.SaveChangesAsync();

            var rolesToRemove = (await userManager.GetRolesAsync(user.Id))
                .Union(new[] { EDUGraphAPI.Constants.Roles.Admin, EDUGraphAPI.Constants.Roles.Faculty, EDUGraphAPI.Constants.Roles.Student })
                .ToArray();

            await userManager.RemoveFromRolesAsync(user.Id, rolesToRemove);
        }

        /// <summary>
        /// Unlink all accounts in the specified tenant
        /// </summary>
        public async Task UnlinkAllAccounts(string tenantId)
        {
            var users = await dbContext.Users
                 .Where(i => i.Organization.TenantId == tenantId)
                 .ToArrayAsync();

            if (users.IsNullOrEmpty())
            {
                return;
            }

            foreach (var user in users)
            {
                user.Organization = null;
                user.O365UserId = null;
                user.O365Email = null;
            }

            dbContext.SaveChanges();
        }

        /// <summary>
        /// Save seating arrangements
        /// </summary>
        public async Task<int> SaveSeatingArrangements(List<SeatingViewModel> seatingArrangements)
        {
            foreach (var item in seatingArrangements)
            {
                var seat = dbContext.ClassroomSeatingArrangements
                    .FirstOrDefault(c => c.O365UserId == item.O365UserId && c.ClassId == item.ClassId);

                //update
                if (seat != null && item.Position != 0)
                {
                    seat.Position = item.Position;
                }

                //delete
                if (seat != null && item.Position == 0)
                {
                    dbContext.ClassroomSeatingArrangements.Remove(seat);
                }

                //insert
                if (seat == null && item.Position != 0)
                {
                    var seating = new ClassroomSeatingArrangements()
                    {
                        O365UserId = item.O365UserId,
                        Position = item.Position,
                        ClassId = item.ClassId
                    };

                    dbContext.ClassroomSeatingArrangements.Add(seating);
                }
            }
            return await dbContext.SaveChangesAsync();
        }

        private string GetUserId()
        {
            var userId = httpContext.User.Identity.GetUserId();

            var aadUserId = httpContext.User.GetObjectIdentifier();
            if (aadUserId.IsNotNullAndEmpty())
            {
                userId = aadUserId;
            }

            return userId;
        }

        private async Task UpdateUserRoles(ApplicationUser localUser, UserInfo o365User)
        {
            var oldRoles = await userManager.GetRolesAsync(localUser.Id);
            var newRoles = o365User.Roles;

            foreach (var role in newRoles.Except(oldRoles))
            {
                if (await dbContext.Roles.AnyAsync(i => i.Name == role))
                {
                    continue;
                }

                dbContext.Roles.Add(new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = role
                });
            }
            await dbContext.SaveChangesAsync();

            await userManager.RemoveFromRolesAsync(localUser.Id, oldRoles.Except(newRoles).ToArray());
            await userManager.AddToRolesAsync(localUser.Id, newRoles.Except(oldRoles).ToArray());
        }

        private async Task UpdateUserNoSaveAsync(ApplicationUser localUser, UserInfo o365User, TenantInfo tenant)
        {
            localUser.FirstName = o365User.GivenName;
            localUser.LastName = o365User.Surname;
            localUser.O365UserId = o365User.Id;
            localUser.O365Email = o365User.Mail ?? o365User.UserPrincipalName;

            // Update organization
            if (localUser.Organization == null || localUser.Organization.TenantId != tenant.Id)
            {
                var organization = await dbContext.Organizations
                    .FirstOrDefaultAsync(i => i.TenantId == tenant.Id);

                if (organization == null)
                {
                    organization = new Organization();
                    InitOrganization(organization, tenant);
                }

                localUser.Organization = organization;
            }
        }

        private void InitOrganization(Organization organization, TenantInfo tenant)
        {
            organization.Created = DateTime.UtcNow;
            organization.Name = tenant.Name;
            organization.TenantId = tenant.Id;
        }

        private async Task<Organization> GetCurrentTenantAsync()
        {
            var tenantId = httpContext.User.GetTenantId();
            if (tenantId == null)
            {
                return null;
            }

            return await dbContext.Organizations
                .FirstOrDefaultAsync(i => i.TenantId == tenantId);
        }
    }
}
