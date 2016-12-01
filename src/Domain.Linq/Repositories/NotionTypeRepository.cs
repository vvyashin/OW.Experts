using System.Linq;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain.Linq
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
