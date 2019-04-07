using System.Reflection;
using Autofac;
using Domain;
using Domain.Infrastructure;
using Domain.Linq;
using Domain.NHibernate;
using Module = Autofac.Module;

namespace WebUI.CompositionRoot.AutofacModules
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var sessionFactory = OWDatabaseConfiguration.Configure();
            builder.RegisterInstance(sessionFactory).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.Load("Domain.NHibernate"))
                .Where(t => t.Name.StartsWith("NH"))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            builder.RegisterGeneric(typeof(NHRepository<>))
                .As(typeof(IRepository<>))
                .Keyed("NHibernate", typeof(IRepository<>))
                .InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.Load("Domain.Linq"))
                .Where(t => t.Name.EndsWith("Repository"))
                .WithParameter(
                    (pi, c) => pi.ParameterType.IsGenericType && pi.ParameterType.GetGenericTypeDefinition() == typeof(IRepository<>),
                    (pi, c) => c.ResolveKeyed("NHibernate", typeof(IRepository<>).MakeGenericType(pi.ParameterType.GetGenericArguments())))
                .AsImplementedInterfaces()
                .InstancePerRequest();

            //builder.RegisterAssemblyTypes(Assembly.Load("Domain.Linq"))
            //    .Where(t => t.Name.EndsWith("Query"))
            //    .AsImplementedInterfaces()
            //    .InstancePerRequest();

            builder.RegisterGeneric(typeof (GetNotionsTypesQuery<>))
                .As(typeof (IGetNotionTypesQuery<>))
                .InstancePerRequest();


            FetchableQueryableFactory.Current = new NHFetchableQueryableFactory();
        }
    }
}