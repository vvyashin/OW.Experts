using System.Reflection;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Module = Autofac.Module;

namespace OW.Experts.WebUI.CompositionRoot.AutofacModules
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.Load("OW.Experts.Domain"))
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.Load("OW.Experts.Domain.Services"))
                .Where(t => t.Name.EndsWith("Service"))
                .FindConstructorsWith(new DefaultConstructorFinder(type =>
                    type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}