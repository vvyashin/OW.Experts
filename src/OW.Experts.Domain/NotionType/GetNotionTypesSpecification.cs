using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public class GetNotionTypesSpecification<TResult>
    {
        public GetNotionTypesSpecification([NotNull] Expression<Func<NotionType, TResult>> projection)
        {
            if (projection == null) throw new ArgumentNullException(nameof(projection));

            Projection = projection;
        }

        public Expression<Func<NotionType, TResult>> Projection { get; }
    }
}