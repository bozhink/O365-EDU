﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Common.Infrastructure
{
    using System;
    using System.Linq;
    using System.Web.Security;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Data.Models;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public class ADALTokenCache : TokenCache
    {
        private static readonly string MachinKeyProtectPurpose = "ADALCache";

        private string userId;

        public ADALTokenCache(string signedInUserId)
        {
            this.userId = signedInUserId;
            this.AfterAccess = this.AfterAccessNotification;
            this.BeforeAccess = this.BeforeAccessNotification;

            this.GetCahceAndDeserialize();
        }

        public override void Clear()
        {
            base.Clear();
            this.ClearUserTokenCache(this.userId);
        }

        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            this.GetCahceAndDeserialize();
        }

        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            if (this.HasStateChanged)
            {
                this.SerializeAndUpdateCache();
                this.HasStateChanged = false;
            }
        }

        private void GetCahceAndDeserialize()
        {
            var cacheBits = this.GetUserTokenCache(this.userId);
            if (cacheBits != null)
            {
                try
                {
                    var data = MachineKey.Unprotect(cacheBits, MachinKeyProtectPurpose);
                    this.Deserialize(data);
                }
                catch
                {
                }
            }
        }

        private void SerializeAndUpdateCache()
        {
            var cacheBits = MachineKey.Protect(this.Serialize(), MachinKeyProtectPurpose);
            this.UpdateUserTokenCache(this.userId, cacheBits);
        }

        private byte[] GetUserTokenCache(string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var cache = this.GetUserTokenCache(db, userId);
                return cache?.CacheBits;
            }
        }

        private void UpdateUserTokenCache(string userId, byte[] cacheBits)
        {
            using (var db = new ApplicationDbContext())
            {
                var cache = this.GetUserTokenCache(db, userId);
                if (cache == null)
                {
                    cache = new UserTokenCache { WebUserUniqueId = userId };
                    db.UserTokenCacheList.Add(cache);
                }

                cache.CacheBits = cacheBits;
                cache.LastWrite = DateTime.UtcNow;

                db.SaveChanges();
            }
        }

        private UserTokenCache GetUserTokenCache(ApplicationDbContext db, string userId)
        {
            return db.UserTokenCacheList
                .OrderByDescending(i => i.LastWrite)
                .FirstOrDefault(c => c.WebUserUniqueId == userId);
        }

        private void ClearUserTokenCache(string userId)
        {
            using (var db = new ApplicationDbContext())
            {
                var cacheEntries = db.UserTokenCacheList
                    .Where(c => c.WebUserUniqueId == userId)
                    .ToArray();

                db.UserTokenCacheList.RemoveRange(cacheEntries);
                db.SaveChanges();
            }
        }
    }
}
