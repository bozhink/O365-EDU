﻿/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Web;
    using EDUGraphAPI.Enumerations;
    using EDUGraphAPI.Services.GraphClients;
    using EDUGraphAPI.Services.Web;
    using EDUGraphAPI.Utils;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Owin.Security;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OpenIdConnect;
    using Owin;

    public partial class Startup
    {
        public void ConfigureAADAuth(IAppBuilder app, IGraphClientFactory graphClientFactory, ICookieService cookieService)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions { });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Caption = "Microsoft Work or school account",
                    ClientId = EDUGraphAPI.Constants.Common.AADClientId,
                    Authority = EDUGraphAPI.Constants.Common.Authority,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            string hint = cookieService.GetCookiesOfEmail();
                            if (!string.IsNullOrEmpty(hint))
                            {
                                context.ProtocolMessage.LoginHint = hint;
                            }

                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = async (context) =>
                        {
                            var identity = context.AuthenticationTicket.Identity;

                            // Get token with authorization code
                            var redirectUri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));
                            var credential = new ClientCredential(EDUGraphAPI.Constants.Common.AADClientId, EDUGraphAPI.Constants.Common.AADClientSecret);
                            var authContext = AuthenticationHelper.GetAuthenticationContext(identity, Permissions.Delegated);
                            var authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(context.Code, redirectUri, credential, EDUGraphAPI.Constants.Resources.AADGraph);

                            // Get user's roles and add them to claims
                            var activeDirectoryClient = authResult.CreateActiveDirectoryClient();
                            var graphClient = graphClientFactory.CreateAADGraphClient(activeDirectoryClient);
                            var user = await graphClient.GetCurrentUserAsync();
                            foreach (var role in user.Roles)
                            {
                                identity.AddClaim(ClaimTypes.Role, role);
                            }
                        },
                        AuthenticationFailed = (context) =>
                        {
                            var redirectUrl = "/Error?message=" + Uri.EscapeDataString(context.Exception.Message);
                            context.OwinContext.Response.Redirect(redirectUrl);
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
