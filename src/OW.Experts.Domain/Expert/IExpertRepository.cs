using System.Collections.Generic;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain
{
    public interface IExpertRepository : IRepository<Expert>
    {
        [CanBeNull]
        Expert GetExpertByNameAndSession([NotNull] GetExpertByNameAndSessionSpecification specification);

        [NotNull]
        IReadOnlyCollection<Expert> GetExpertsBySession([NotNull] GetExpertsBySessionSpecification specification);
    }
}