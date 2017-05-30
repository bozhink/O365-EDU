/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Services.Models.Web
{
    using System.Collections.Generic;

    public class DemoPage
    {
        public DemoPage()
        {
            this.Links = new HashSet<Link>();
        }

        public string Action { get; set; }

        public string Controller { get; set; }

        public ISet<Link> Links { get; set; }
    }
}
