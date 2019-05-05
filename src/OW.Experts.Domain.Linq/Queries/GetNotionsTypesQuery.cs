using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;

namespace OW.Experts.Domain.Linq.Queries
{
    public class GetNotionsTypesQuery<TResultItem>
        : LinqQueryBase<IReadOnlyCollection<TResultItem>, GetNotionTypesSpecification<TResultItem>>,
            IGetNotionTypesQuery<TResultItem>
    {
        public GetNotionsTypesQuery([NotNull] ILinqProvider lingProvider)
            : base(lingProvider)
        {
        }

        public override IReadOnlyCollection<TResultItem> Execute(GetNotionTypesSpecification<TResultItem> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return LingProvider.Query<NotionType>().Select(specification.Projection).ToList();
        }
    }
}