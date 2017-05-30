/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Data
{
    using System.Data.Entity;
    using EDUGraphAPI.Data.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : this("DefaultConnection")
        {
        }

        public ApplicationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString, throwIfV1Schema: false)
        {
        }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }

        public DbSet<DataSyncRecord> DataSyncRecords { get; set; }

        public DbSet<ClassroomSeatingArrangements> ClassroomSeatingArrangements { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
