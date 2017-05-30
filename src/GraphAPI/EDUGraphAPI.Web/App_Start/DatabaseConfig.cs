namespace EDUGraphAPI.Web
{
    using EDUGraphAPI.Data;

    public class DatabaseConfig
    {
        public static void RegisterDatabases()
        {
            DatabseInitializer.Initialize();
        }
    }
}
