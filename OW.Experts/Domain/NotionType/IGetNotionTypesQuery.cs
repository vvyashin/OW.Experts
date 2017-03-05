using System.Collections.Generic;
using Domain.Infrastructure;

namespace Domain
{
    // alias
    public interface IGetNotionTypesQuery<TResultItem> : IQuery<IReadOnlyCollection<TResultItem>, GetNotionTypesSpecification<TResultItem>>
    {
    }
}
