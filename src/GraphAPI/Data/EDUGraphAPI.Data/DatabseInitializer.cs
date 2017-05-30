namespace EDUGraphAPI.Data
{
    using System.Data.Entity;
    using EDUGraphAPI.Data.Migrations;

    public static class DatabseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }
    }
}
