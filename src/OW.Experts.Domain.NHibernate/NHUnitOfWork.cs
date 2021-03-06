﻿using System;
using System.Data;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Context;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.NHibernate
{
    public class NHUnitOfWork : IUnitOfWork
    {
        private readonly bool isCommited = false;
        private ISession _session;
        private ITransaction _transaction;

        internal NHUnitOfWork([NotNull] ISessionFactory sessionFactory, IsolationLevel isolationLevel)
        {
            if (sessionFactory == null) throw new ArgumentNullException(nameof(sessionFactory));
            if (CurrentSessionContext.HasBind(sessionFactory))
                throw new InvalidOperationException("UnitOfWork already created. Please, use or dispose it");

            _session = sessionFactory.OpenSession();
            CurrentSessionContext.Bind(_session);
            _transaction = _session.BeginTransaction(isolationLevel);
        }

        public void Dispose()
        {
            if (!_transaction.WasCommitted && !_transaction.WasRolledBack)
                _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;

            CurrentSessionContext.Unbind(_session.SessionFactory);
            _session.Dispose();
            _session = null;
        }

        public void Commit()
        {
            if (isCommited)
                throw new InvalidOperationException("Transaction was commited. Please, create new UnitOfWork.");

            if (_transaction == null)
                throw new InvalidOperationException("Transaction was rolled back. Please, create new UnitOfWork.");

            _transaction.Commit();
        }
    }
}