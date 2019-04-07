using System;

namespace OW.Experts.Domain.Infrastructure.Repository
{
    /// <summary>
    /// Wrapper class for managing transaction
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}