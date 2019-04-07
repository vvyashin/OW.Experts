// Credit to http://stackoverflow.com/users/718670/guido and http://stackoverflow.com/users/647901/seldon
// http://stackoverflow.com/a/5742158

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using OW.Experts.Domain.Infrastructure.Fetching;

namespace OW.Experts.Domain.NHibernate
{
    public class NHFetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
    {

        public NHFetchRequest(INhFetchRequest<TQueried, TFetch> nhFetchRequest)
        {
            NhFetchRequest = nhFetchRequest;
        }

        public INhFetchRequest<TQueried, TFetch> NhFetchRequest { get; private set; }

        #region IEnumerable<TQueried> Members

        public IEnumerator<TQueried> GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return NhFetchRequest.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return NhFetchRequest.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return NhFetchRequest.Provider; }
        }

        #endregion

        #region IFetchRequest<TQueried,TFetch> Members

        public IFetchRequest<TQueried, TRelated> Fetch<TRelated>(Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.Fetch(this.NhFetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> FetchMany<TRelated>(Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fecth = EagerFetchingExtensionMethods.FetchMany(this.NhFetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fecth);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetch<TRelated>(Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.ThenFetch(this.NhFetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetchMany<TRelated>(Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.ThenFetchMany(this.NhFetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IEnumerable<TQueried> ToFuture()
        {
            return this.NhFetchRequest.ToFuture();
        }

        #endregion
    }
}
