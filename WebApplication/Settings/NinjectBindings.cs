namespace WebApplication.Settings
{
    using Ninject.Extensions.Conventions;
    using Ninject.Modules;

    public class NinjectBindings : NinjectModule
    {
        public override void Load()
        {
            this.Bind(configure =>
            {
                configure.FromThisAssembly()
                    .SelectAllClasses()
                    .BindDefaultInterface();
            });
        }
    }
}
