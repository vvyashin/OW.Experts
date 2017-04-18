using JetBrains.Annotations;

namespace Domain
{
    public interface IRelationTypeRepository : ITypeRepository<RelationType>
    {
        [NotNull]
        RelationType GetTaxonomyType();

        [NotNull]
        RelationType GetMeronomyType();
    }
}
