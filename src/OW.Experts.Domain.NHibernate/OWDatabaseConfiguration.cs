using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace OW.Experts.Domain.NHibernate
{
    public static class OWDatabaseConfiguration
    {
        public static ISessionFactory Configure(string currentSessionContext = "web")
        {
            return Fluently.Configure().Database(MsSqlConfiguration.MsSql2012.ConnectionString(
                        @"Data Source=(localdb)\ProjectsV13;Initial Catalog=OW;Integrated Security=True;")
                    .ShowSql)
                .CurrentSessionContext(currentSessionContext)
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHUnitOfWork>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory();
        }
    }
}
