using System;
using System.Configuration;

namespace WebApplication.Settings
{
    public class SettingsHelper
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"] ?? ConfigurationManager.AppSettings["ida:AppKey"] ?? ConfigurationManager.AppSettings["ida:Password"];

        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string authorizationUri = "https://login.windows.net";
        private static string authority = "https://login.windows.net/{0}/";

        private static string graphResourceId = "https://graph.windows.net";
        private static string discoverySvcResourceId = "https://api.office.com/discovery/";
        private static string discoverySvcEndpointUri = "https://api.office.com/discovery/v1.0/me/";

        public static string ClientId => clientId;

        public static string AppKey => appKey;

        public static string TenantId => tenantId;

        public static string AuthorizationUri => authorizationUri;

        public static string Authority => string.Format(authority, tenantId);

        public static string AADGraphResourceId => graphResourceId;

        public static string DiscoveryServiceResourceId => discoverySvcResourceId;

        public static Uri DiscoveryServiceEndpointUri => new Uri(discoverySvcEndpointUri);
    }
}
