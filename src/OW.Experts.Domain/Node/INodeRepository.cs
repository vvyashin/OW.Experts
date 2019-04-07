using System.Collections.Generic;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain
{
    public interface INodeRepository : IRepository<Node>
    {
        [CanBeNull]
        Node GetByNotionAndType([NotNull] string notion, [NotNull] NotionType type);

        [NotNull]
        IReadOnlyCollection<Node> GetBySession([NotNull] SessionOfExperts session);

        [NotNull]
        SemanticNetworkReadModel GetSemanticNetworkBySession([NotNull] SessionOfExperts session);
    }
}
