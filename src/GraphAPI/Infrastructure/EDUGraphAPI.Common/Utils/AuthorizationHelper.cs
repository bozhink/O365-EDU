﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Utils
{
    using System;

    /// <summary>
    /// A static class used to build authorize URL
    /// </summary>
    public static class AuthorizationHelper
    {
        public static string GetUrl(string redirectUrl, string state, string resource, string prompt = null)
        {
            var url = string.Format(
                "{0}oauth2/authorize?response_type=code&client_id={1}&resource={2}&redirect_uri={3}&state={4}",
                EDUGraphAPI.Constants.Common.Authority,
                Uri.EscapeDataString(EDUGraphAPI.Constants.Common.AADClientId),
                Uri.EscapeDataString(resource),
                Uri.EscapeDataString(redirectUrl),
                Uri.EscapeDataString(state)
            );

            if (prompt.IsNotNullAndEmpty())
            {
                url += "&prompt=" + Uri.EscapeDataString(prompt);
            }

            return url;
        }
    }
}
