namespace WebApplication.Settings
{
    using Ninject.Extensions.Conventions;
    using Ninject.Modules;
    using Ninject.Web.Common;
    using Services;

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

            this.Bind<IAuthProvider>()
                .To<SampleAuthProvider>()
                .InRequestScope();
        }
    }
}
