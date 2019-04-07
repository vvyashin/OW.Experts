using System.Data;
using JetBrains.Annotations;

namespace OW.Experts.Domain.Infrastructure.Repository
{
    public interface IUnitOfWorkFactory
    {
        [NotNull]
        IUnitOfWork Create(IsolationLevel isolationLevel);

        [NotNull]
        IUnitOfWork Create();
    }
}