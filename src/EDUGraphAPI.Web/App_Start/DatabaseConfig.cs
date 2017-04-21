using EDUGraphAPI.Data;

namespace EDUGraphAPI.Web
{
    public class DatabaseConfig
    {
        public static void RegisterDatabases()
        {
            DatabseInitializer.Initialize();
        }
    }
}
