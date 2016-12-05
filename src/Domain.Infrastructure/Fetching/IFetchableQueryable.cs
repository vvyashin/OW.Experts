// Credit to http://stackoverflow.com/users/718670/guido and http://stackoverflow.com/users/647901/seldon
// http://stackoverflow.com/a/5742158

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Infrastructure
{
    public interface IFetchableQueryable<TQueried> : IQueryable<TQueried>
    {
        IFetchRequest<TQueried, TRelated> Fetch<TRelated>(Expression<Func<TQueried, TRelated>> relatedObjectSelector);

        IFetchRequest<TQueried, TRelated> FetchMany<TRelated>(Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector);
    }
}
