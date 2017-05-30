﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Data.Models
{
    using System;

    public class DataSyncRecord
    {
        public int Id { get; set; }

        public string TenantId { get; set; }

        public string Query { get; set; }

        public string DeltaLink { get; set; }

        public DateTime Updated { get; set; }
    }
}
