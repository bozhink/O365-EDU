using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using Microsoft.Office365.OutlookServices;
using WebApplication.Models;
using WebApplication.Settings;

namespace WebApplication.Factories
{
    public class ClientFactory
    {
        public static async Task<OutlookServicesClient> GreateOutlookServicesClientAsync(string capability)
        {
            if (string.IsNullOrWhiteSpace(capability))
            {
                throw new ArgumentNullException(nameof(capability));
            }

            var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            var authenticationContext = new AuthenticationContext(SettingsHelper.Authority, new ADALTokenCache(signInUserId));

            try
            {
                var discoveryClient = new DiscoveryClient(
                    SettingsHelper.DiscoveryServiceEndpointUri,
                    async () =>
                    {
                        var authenticationResult = await authenticationContext.AcquireTokenSilentAsync(
                            SettingsHelper.DiscoveryServiceResourceId,
                            new ClientCredential(
                                SettingsHelper.ClientId,
                                SettingsHelper.AppKey),
                            new UserIdentifier(
                                userObjectId,
                                UserIdentifierType.UniqueId));

                        return authenticationResult.AccessToken;
                    });

                var capabilityDiscoveryResult = await discoveryClient.DiscoverCapabilityAsync(capability);

                var client = new OutlookServicesClient(
                    capabilityDiscoveryResult.ServiceEndpointUri,
                    async () =>
                    {
                        var authResult = await authenticationContext.AcquireTokenSilentAsync(
                            capabilityDiscoveryResult.ServiceResourceId,
                            new ClientCredential(
                                SettingsHelper.ClientId,
                                SettingsHelper.AppKey),
                            new UserIdentifier(
                                userObjectId,
                                UserIdentifierType.UniqueId));

                        return authResult.AccessToken;
                    });

                return client;
            }
            catch (AdalException exception)
            {
                //handle token acquisition failure
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    authenticationContext.TokenCache.Clear();

                    //handle token acquisition failure
                }

                throw exception;
            }
            catch
            {
                throw;
            }
        }
    }
}
