namespace EDUGraphAPI.Constants
{
    using System.Configuration;

    public sealed class ConfigurationConstants
    {
        public static readonly string CertPath = ConfigurationManager.AppSettings["CertPath"];
        public static readonly string CertPassword = ConfigurationManager.AppSettings["CertPassword"];
    }
}
