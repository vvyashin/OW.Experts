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
    public class NHFetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
    {
        public NHFetchRequest(INhFetchRequest<TQueried, TFetch> nhFetchRequest)
        {
            NhFetchRequest = nhFetchRequest;
        }

        public INhFetchRequest<TQueried, TFetch> NhFetchRequest { get; }

        public Type ElementType => NhFetchRequest.ElementType;

        public Expression Expression => NhFetchRequest.Expression;

        public IQueryProvider Provider => NhFetchRequest.Provider;

        public IEnumerator<TQueried> GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        public IFetchRequest<TQueried, TRelated> Fetch<TRelated>(
            Expression<Func<TQueried, TRelated>> relatedObjectSelector)
        {
            var fetch = EagerFetchingExtensionMethods.Fetch(NhFetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> FetchMany<TRelated>(
            Expression<Func<TQueried, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fecth = EagerFetchingExtensionMethods.FetchMany(NhFetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fecth);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetch<TRelated>(
            Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            var fetch = NhFetchRequest.ThenFetch(relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetchMany<TRelated>(
            Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            var fetch = NhFetchRequest.ThenFetchMany(relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IEnumerable<TQueried> ToFuture()
        {
            return NhFetchRequest.ToFuture();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }
    }
}