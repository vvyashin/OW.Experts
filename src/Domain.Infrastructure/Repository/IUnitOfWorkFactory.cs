using System.Data;
using JetBrains.Annotations;

namespace Domain.Infrastructure
{
    public interface IUnitOfWorkFactory
    {
        [NotNull]
        IUnitOfWork Create(IsolationLevel isolationLevel);

        [NotNull]
        IUnitOfWork Create();
    }
}