/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.Models.DifferentialQuery
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;

    public class DeltaResult<TEntity>
        where TEntity : class
    {
        [JsonProperty("aad.deltaLink")]
        public string DeltaLink { get; set; }

        [JsonProperty("aad.nextLink")]
        public string NextLink { get; set; }

        [JsonProperty("value")]
        public TEntity[] Items { get; set; }

        public DeltaResult<TTarget> Convert<TTarget>()
            where TTarget : class
        {
            return new DeltaResult<TTarget>
            {
                DeltaLink = this.DeltaLink,
                NextLink = this.NextLink,
                Items = this.Items.OfType<TTarget>().ToArray()
            };
        }

        public DeltaResult<TTarget> Convert<TTarget>(Func<TEntity, TTarget> selector)
            where TTarget : class
        {
            return new DeltaResult<TTarget>
            {
                DeltaLink = this.DeltaLink,
                NextLink = this.NextLink,
                Items = this.Items.Select(selector).ToArray()
            };
        }
    }
}
