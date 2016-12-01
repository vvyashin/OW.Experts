using System.Linq;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain.Linq
{
    public class RelationTypeRepository : TypeRepository<RelationType>, IRelationTypeRepository
    {
        public RelationTypeRepository([NotNull] IRepository<RelationType> repository, [NotNull] ILinqProvider linqProvider) : 
            base(repository, linqProvider)
        {
        }

        public override RelationType GetGeneralType()
        {
            return LinqProvider.Query<RelationType>().Single(x => x.Name == Constants.GeneralRelationType);
        }

        public RelationType GetTaxonomyType()
        {
            return LinqProvider.Query<RelationType>().Single(x => x.Name == Constants.TaxonomyType);
        }

        public RelationType GetMeronomyType()
        {
            return LinqProvider.Query<RelationType>().Single(x => x.Name == Constants.MeronomyType);
        }
    }
}
