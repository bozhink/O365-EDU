using EDUGraphAPI.Migrations;
using System.Data.Entity;

namespace EDUGraphAPI.Data
{
    public static class DatabseInitializer
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        }
    }
}
