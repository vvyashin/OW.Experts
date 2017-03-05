using System;
using Domain.Infrastructure;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Context;

namespace Domain.NHibernate
{
    public class NHRepository<T> : IRepository<T>
        where T : DomainObject
    {
        private readonly ISessionFactory _sessionFactory;

        public NHRepository([NotNull] ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException(nameof(sessionFactory));

            _sessionFactory = sessionFactory;
        }

        private ISession GetSession()
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory)) {
                throw new InvalidOperationException("Unit of work should be started to using repository");
            }

            return _sessionFactory.GetCurrentSession();
        }

        public void AddOrUpdate(T entity)
        {
            var session = GetSession();
            session.SaveOrUpdate(entity);
        }

        public T GetById(Guid id)
        {
            var session = GetSession();

            T @return;
            if ((@return = session.Get<T>(id) ) == null) {
                throw new InvalidOperationException($"Entity of type {typeof(T).FullName} with Id = {id} does not exist");
            }
            return @return;
        }
        
        public void Remove(T entity)
        {
            var session = GetSession();

            session.Delete(entity);
        }
    }
}
