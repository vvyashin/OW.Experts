using System.Collections.Generic;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain
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
