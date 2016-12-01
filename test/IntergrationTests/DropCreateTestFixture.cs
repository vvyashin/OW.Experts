using Domain.Infrastructure;
using Domain.NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace IntergrationTests
{
    public class DropCreateTestFixture
    {
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }
        public ILinqProvider LinqProvider { get; private set; }
        private ISessionFactory SessionFactory { get; set; }

        protected IRepository<T> GetRepository<T>()
            where T : DomainObject
        {
            return new NHRepository<T>(SessionFactory);
        }

        [OneTimeSetUp]
        public void RegisterDependencies()
        {
            FetchableQueryableFactory.Current = new NHFetchableQueryableFactory();
        }

        public void DropCreate()
        {
            SessionFactory = Fluently.Configure().Database(MsSqlConfiguration.MsSql2012.ConnectionString(
                        @"Data Source=(localdb)\ProjectsV13;Initial Catalog=TestOW;Integrated Security=True;")
                    .ShowSql)
                .CurrentSessionContext("thread_static")
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHUnitOfWork>())
                .ExposeConfiguration(cfg =>
                {
                    // logging in output window of test
                    cfg.SetInterceptor(new SqlStatementInterceptor());

                    var schema = new SchemaExport(cfg);
                    schema.Drop(false, true);
                    schema.Create(false, true);
                })
                .BuildSessionFactory();

            UnitOfWorkFactory = new NHUnitOfWorkFactory(SessionFactory);
            LinqProvider = new NHLinqProvider(SessionFactory);
        }
    }
}
