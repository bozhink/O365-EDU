namespace EDUGraphAPI.Data.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
#if DEBUG
            this.AutomaticMigrationDataLossAllowed = true;
#endif
            this.ContextType = typeof(ApplicationDbContext);
            this.ContextKey = this.ContextType.FullName;
        }

        protected override void Seed(ApplicationDbContext context)
        {
        }
    }
}
