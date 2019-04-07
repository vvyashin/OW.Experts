using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using OW.Experts.WebUI.CompositionRoot.AutofacModules;
using OW.Experts.WebUI.Services;

namespace OW.Experts.WebUI.CompositionRoot
{
    public static class AutofacConfig
    {
        public static void RegisterDependency()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(Assembly.Load("OW.Experts.WebUI"));
            builder.RegisterFilterProvider();
            builder.RegisterType<LogService>().AsSelf().SingleInstance();
            builder.RegisterModule(new DataAccessModule());
            builder.RegisterModule(new DomainModule());
            
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}