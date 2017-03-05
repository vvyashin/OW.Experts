// Credit to http://stackoverflow.com/users/718670/guido and http://stackoverflow.com/users/647901/seldon
// http://stackoverflow.com/a/5742158

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Infrastructure
{
    public interface IFetchRequest<TQueried, TFetch> : IOrderedQueryable<TQueried>
    {
        IFetchRequest<TQueried, TRelated> ThenFetch<TRelated>(Expression<Func<TFetch, TRelated>> relatedObjectSelector);

        IFetchRequest<TQueried, TRelated> ThenFetchMany<TRelated>(Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector);

        IEnumerable<TQueried> ToFuture();
    }
}
