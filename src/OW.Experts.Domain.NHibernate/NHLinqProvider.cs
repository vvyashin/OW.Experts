using System;
using System.Linq;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Context;
using OW.Experts.Domain.Infrastructure;
using OW.Experts.Domain.Infrastructure.Query;

namespace OW.Experts.Domain.NHibernate
{
    public class NHLinqProvider : ILinqProvider
    {
        private readonly ISessionFactory _sessionFactory;

        public NHLinqProvider([NotNull] ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException(nameof(sessionFactory));

            _sessionFactory = sessionFactory;
        }

        public IQueryable<T> Query<T>()
            where T : DomainObject
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory))
                throw new InvalidOperationException("Unit of work should be started to query database");
            return _sessionFactory.GetCurrentSession().Query<T>();
        }
    }
}