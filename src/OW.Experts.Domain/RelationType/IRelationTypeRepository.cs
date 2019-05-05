using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public interface IRelationTypeRepository : ITypeRepository<RelationType>
    {
        [NotNull]
        RelationType GetTaxonomyType();

        [NotNull]
        RelationType GetMeronomyType();
    }
}