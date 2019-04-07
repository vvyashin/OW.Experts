using System.Linq;
using JetBrains.Annotations;

namespace OW.Experts.Domain.Infrastructure.Query
{
    public interface ILinqProvider
    {
        [NotNull]
        IQueryable<T> Query<T>() where T : DomainObject;
    }
}
