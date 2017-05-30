/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.Models.DifferentialQuery
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public interface IDeltaEntity
    {
        [JsonProperty("aad.isDeleted")]
        bool IsDeleted { get; set; }

        HashSet<string> ModifiedPropertyNames { get; }
    }
}
