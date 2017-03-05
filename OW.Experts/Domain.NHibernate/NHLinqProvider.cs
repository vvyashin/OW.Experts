using System;
using System.Linq;
using Domain.Infrastructure;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Context;
using NHibernate.Linq;

namespace Domain.NHibernate
{
    public class NHLinqProvider : ILinqProvider
    {
        private readonly ISessionFactory _sessionFactory;
        
        public NHLinqProvider([NotNull] ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException(nameof(sessionFactory));

            _sessionFactory = sessionFactory;
        }

        public IQueryable<T> Query<T>() where T : DomainObject
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory)) {
                throw new InvalidOperationException("Unit of work should be started to query database");
            }
            else {
                return _sessionFactory.GetCurrentSession().Query<T>();
            }
        }
    }
}
