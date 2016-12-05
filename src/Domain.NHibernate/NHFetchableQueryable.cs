// Credit to http://stackoverflow.com/users/718670/guido and http://stackoverflow.com/users/647901/seldon
// http://stackoverflow.com/a/5742158

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Domain.Infrastructure;
using NHibernate.Linq;

namespace Domain.NHibernate
{
    public class NHFetchableQueryable<TQueried> : IFetchableQueryable<TQueried>
    {
        public NHFetchableQueryable(IQueryable<TQueried> query)
        {
            this.Query = query;
        }

        public IQueryable<TQueried> Query { get; private set; }

        #region IEnumerable<TQueried> Members

        public IEnumerator<TQueried> GetEnumerator()
        {
            return this.Query.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Query.GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return this.Query.ElementType; }
        }

        public Expression Expression
        {
            get { return this.Query.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return this.Query.Provider; }
        }

        #endregion

        #region IFetchableQueryable<TQueried> Members

        public IFetchRequest<TQueried, TRelated> Fetch<TRelated>(Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            return new NHFetchRequest<TQueried, TRelated>(EagerFetchingExtensionMethods.Fetch<TQueried, TRelated>(this.Query, relatedObjectSelector));
        }

        public IFetchRequest<TQueried, TRelated> FetchMany<TRelated>(Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return new NHFetchRequest<TQueried, TRelated>(EagerFetchingExtensionMethods.FetchMany<TQueried, TRelated>(this.Query, relatedObjectSelector));
        }

        #endregion
    }
}
