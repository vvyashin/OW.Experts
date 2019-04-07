using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain
{
    public interface IVergeRepository : IRepository<Verge>
    {
        [CanBeNull]
        Verge GetByNodesAndTypes(Node source, Node destination, RelationType type);
    }
}
