using System.Linq;
using OW.Experts.Domain.Infrastructure.Fetching;

namespace OW.Experts.Domain.NHibernate
{
    public class NHFetchableQueryableFactory : FetchableQueryableFactory
    {
        public override IFetchableQueryable<T> GetFetchable<T>(IQueryable<T> query)
        {
            return new NHFetchableQueryable<T>(query);
        }
    }
}
