using System.Linq;
using Domain.Infrastructure;

namespace Domain.NHibernate
{
    public class NHFetchableQueryableFactory : FetchableQueryableFactory
    {
        public override IFetchableQueryable<T> GetFetchable<T>(IQueryable<T> query)
        {
            return new NHFetchableQueryable<T>(query);
        }
    }
}
