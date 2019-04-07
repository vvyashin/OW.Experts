using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OW.Experts.Domain.Infrastructure.Fetching
{
    public static class FetchingExtensions
    {
        public static IFetchRequest<T, TFetch> Fetch<T, TFetch>(this IQueryable<T> query, Expression<Func<T, TFetch>> relatedObjectSelector)
        {
            return FetchableQueryableFactory.Current.GetFetchable(query).Fetch(relatedObjectSelector);
        }

        public static IFetchRequest<T, TRelated> FetchMany<T, TRelated>(this IQueryable<T> query, 
            Expression<Func<T, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return FetchableQueryableFactory.Current.GetFetchable(query).FetchMany(relatedObjectSelector);
        }
    }
}
