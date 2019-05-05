// Credit to http://stackoverflow.com/users/718670/guido and http://stackoverflow.com/users/647901/seldon
// http://stackoverflow.com/a/5742158

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using OW.Experts.Domain.Infrastructure.Fetching;

namespace OW.Experts.Domain.NHibernate
{
    public class NHFetchableQueryable<TQueried> : IFetchableQueryable<TQueried>
    {
        public NHFetchableQueryable(IQueryable<TQueried> query)
        {
            Query = query;
        }

        public IQueryable<TQueried> Query { get; }

        #region IQueryable Members

        public Type ElementType => Query.ElementType;

        public Expression Expression => Query.Expression;

        public IQueryProvider Provider => Query.Provider;

        #endregion

        #region IEnumerable<TQueried> Members

        public IEnumerator<TQueried> GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Query.GetEnumerator();
        }

        #endregion

        #region IFetchableQueryable<TQueried> Members

        public IFetchRequest<TQueried, TRelated> Fetch<TRelated>(
            Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            return new NHFetchRequest<TQueried, TRelated>(
                EagerFetchingExtensionMethods.Fetch(Query, relatedObjectSelector));
        }

        public IFetchRequest<TQueried, TRelated> FetchMany<TRelated>(
            Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return new NHFetchRequest<TQueried, TRelated>(
                EagerFetchingExtensionMethods.FetchMany(Query, relatedObjectSelector));
        }

        #endregion
    }
}