using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.Linq.Repositories
{
    public class NotionTypeRepository : TypeRepository<NotionType>
    {
        public NotionTypeRepository([NotNull] IRepository<NotionType> repository, [NotNull] ILinqProvider linqProvider) : 
            base(repository, linqProvider)
        {
        }

        public override NotionType GetGeneralType()
        {
            return LinqProvider.Query<NotionType>().Single(x => x.Name == Constants.GeneralNotionType);
        }
    }
}
