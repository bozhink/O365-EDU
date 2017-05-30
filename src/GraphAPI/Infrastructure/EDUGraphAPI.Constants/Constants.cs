namespace EDUGraphAPI.Constants
{
    using System.Collections.Generic;
    using System.Configuration;

    public sealed class Common
    {
        public const string AADInstance = "https://login.microsoftonline.com/";
        public const string Authority = AADInstance + "common/";

        public const string GraphResourceRootUrl = "https://graph.windows.net";

        public const string O365GroupConversationsUrl = "https://outlook.office.com/owa/?path=/group/{0}/mail&exsvurl=1&ispopout=0";

        public const string AADCompanyAdminRoleName = "Company Administrator";

        public const string UsernameCookie = "O365CookieUsername";
        public const string EmailCookie = "O365CookieEmail";

        public static readonly string AADClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static readonly string AADClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];

        public static readonly string BingMapKey = ConfigurationManager.AppSettings["BingMapKey"];

        public static readonly string SourceCodeRepositoryUrl = ConfigurationManager.AppSettings["SourceCodeRepositoryUrl"];

        public static readonly List<ColorEntity> FavoriteColors = new List<ColorEntity>
        {
            new ColorEntity
            {
                DisplayName = "Blue",
                Value = "#2F19FF"
            },
            new ColorEntity
            {
                DisplayName = "Green",
                Value = "#127605"
            },
            new ColorEntity
            {
                DisplayName = "Grey",
                Value = "#535353"
            }
        };
    }
}
