using System.Collections.Generic;
using OW.Experts.Domain.Infrastructure.Query;

namespace OW.Experts.Domain
{
    // alias
    public interface IGetNotionTypesQuery<TResultItem> : IQuery<IReadOnlyCollection<TResultItem>, GetNotionTypesSpecification<TResultItem>>
    {
    }
}
