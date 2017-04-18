using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain
{
    public interface IVergeRepository : IRepository<Verge>
    {
        [CanBeNull]
        Verge GetByNodesAndTypes(Node source, Node destination, RelationType type);
    }
}
