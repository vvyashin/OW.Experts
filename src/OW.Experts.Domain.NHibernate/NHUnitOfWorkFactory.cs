using System;
using System.Data;
using JetBrains.Annotations;
using NHibernate;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.NHibernate
{
    public class NHUnitOfWorkFactory : IUnitOfWorkFactory
    {
        [NotNull]
        private readonly ISessionFactory _sessionFactory;

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