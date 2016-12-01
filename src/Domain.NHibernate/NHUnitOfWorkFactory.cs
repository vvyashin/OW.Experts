using System;
using System.Data;
using Domain.Infrastructure;
using JetBrains.Annotations;
using NHibernate;

namespace Domain.NHibernate
{
    public class NHUnitOfWorkFactory : IUnitOfWorkFactory
    {
        [NotNull]
        private ISessionFactory _sessionFactory;

        public NHUnitOfWorkFactory([NotNull] ISessionFactory sessionFactory)
        {
            if (sessionFactory == null) throw new ArgumentNullException(nameof(sessionFactory));

            _sessionFactory = sessionFactory;
        }

        public IUnitOfWork Create(IsolationLevel isolationLevel)
        {
            return new NHUnitOfWork(_sessionFactory, isolationLevel);
        }

        public IUnitOfWork Create()
        {
            return Create(IsolationLevel.ReadCommitted);
        }
    }
}
