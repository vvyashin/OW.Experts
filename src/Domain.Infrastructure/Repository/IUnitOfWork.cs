using System;

namespace Domain.Infrastructure
{
    /// <summary>
    /// Wrapper class for managing transaction
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}