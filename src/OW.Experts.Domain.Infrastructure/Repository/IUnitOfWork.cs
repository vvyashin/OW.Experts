using System;

namespace OW.Experts.Domain.Infrastructure.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
    }
}