/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace EDUGraphAPI.Data
{
    public class UserTokenCache
    {
        [Key]
        public int UserTokenCacheId { get; set; }

        public string WebUserUniqueId { get; set; }

        [MaxLength]
        public byte[] CacheBits { get; set; }

        public DateTime LastWrite { get; set; }
    }
}
