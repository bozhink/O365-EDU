/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Enumerations
{
    public enum Permissions
    {
        /// <summary>
        /// The client accesses the web API as the signed-in user.
        /// </summary>
        Delegated,

        /// <summary>
        /// The client accesses the web API directly as itself (no user context).
        /// </summary>
        /// <remarks>
        /// This type of permission requires administrator consent.
        /// </remarks>
        Application
    }
}
