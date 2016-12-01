using System.Linq;

namespace Domain.Infrastructure
{
    public abstract class FetchableQueryableFactory
    {
        public static FetchableQueryableFactory Current { get; set; }

        public abstract IFetchableQueryable<T> GetFetchable<T>(IQueryable<T> query);
    }
}
