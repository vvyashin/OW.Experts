// Credit to http://stackoverflow.com/users/718670/guido and http://stackoverflow.com/users/647901/seldon
// http://stackoverflow.com/a/5742158

using System.Linq;

namespace OW.Experts.Domain.Infrastructure.Fetching
{
    public abstract class FetchableQueryableFactory
    {
        public static FetchableQueryableFactory Current { get; set; }

        public abstract IFetchableQueryable<T> GetFetchable<T>(IQueryable<T> query);
    }
}