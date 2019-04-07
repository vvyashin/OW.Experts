using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.Linq.Repositories
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
