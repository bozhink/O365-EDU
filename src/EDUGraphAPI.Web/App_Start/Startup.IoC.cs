/*
 *   * Copyright (c) Microsoft Corporation. All rights reserved. Licensed under the MIT license.
 *   * See LICENSE in the project root for license information.
 */

namespace EDUGraphAPI.Web
{
    using System.Web;
    using System.Web.Mvc;
    using Autofac;
    using Autofac.Integration.Mvc;
    using EDUGraphAPI.Data;
    using EDUGraphAPI.Services.Web;
    using EDUGraphAPI.Web.Controllers;
    using EDUGraphAPI.Web.Services;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Education.Services;
    using EDUGraphAPI.Services.BingMaps;
    using Owin;

    public partial class Startup
    {
        public void ConfigureIoC(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            this.Register(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // OWIN MVC SETUP:
            // Register the Autofac middleware FIRST, then the Autofac MVC middleware.
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
        }

        private void Register(ContainerBuilder builder)
        {
            builder.RegisterControllers(typeof(HomeController).Assembly);
            builder.RegisterType<BingMapService>().As<IBingMapService>().InstancePerRequest();
            builder.RegisterType<ApplicationService>().As<IApplicationService>().InstancePerRequest();
            builder.RegisterType<SchoolsService>().As<ISchoolsService>().InstancePerRequest();
            builder.RegisterType<SchoolsServiceFactory>().As<ISchoolsServiceFactory>().InstancePerRequest();
            builder.RegisterType<DemoHelperService>().AsSelf().InstancePerRequest();
            builder.RegisterType<CookieService>().AsSelf().InstancePerRequest();

            builder.RegisterType<EducationServiceClient>().As<IEducationServiceClient>().InstancePerRequest();

            // The following types are registered in Startup.Auth.Identity.cs
            // app.CreatePerOwinContext(ApplicationDbContext.Create);
            // app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            // app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            builder.Register(c => HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>());
            builder.Register(c => HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>());
            builder.Register(c => HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>());
        }
    }
}
