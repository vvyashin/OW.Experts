using System.Collections.Generic;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain
{
    public interface IExpertRepository : IRepository<Expert>
    {
        [CanBeNull]
        Expert GetExpertByNameAndSession([NotNull] GetExpertByNameAndSessionSpecification specification);

        [NotNull]
        IReadOnlyCollection<Expert> GetExpertsBySession([NotNull] GetExpertsBySessionSpecification specification);
    }
}