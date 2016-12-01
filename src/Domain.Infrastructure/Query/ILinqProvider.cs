using System.Linq;
using JetBrains.Annotations;

namespace Domain.Infrastructure
{
    public interface ILinqProvider
    {
        [NotNull]
        IQueryable<T> Query<T>() where T : DomainObject;
    }
}
