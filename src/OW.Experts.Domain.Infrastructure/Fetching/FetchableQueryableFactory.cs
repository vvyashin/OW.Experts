using System.Linq;

namespace OW.Experts.Domain.Infrastructure.Fetching
{
    public abstract class FetchableQueryableFactory
    {
        public static FetchableQueryableFactory Current { get; set; }

        public abstract IFetchableQueryable<T> GetFetchable<T>(IQueryable<T> query);
    }
}
