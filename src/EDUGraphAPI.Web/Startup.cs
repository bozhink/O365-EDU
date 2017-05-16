/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web
{
    using EDUGraphAPI.Services.GraphClients;
    using Owin;

    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureIdentityAuth(app);
            ConfigureAADAuth(app, new GraphClientFactory());
            ConfigureIoC(app);
        }
    }
}
