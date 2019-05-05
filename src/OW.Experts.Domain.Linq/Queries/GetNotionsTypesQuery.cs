using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;

namespace OW.Experts.Domain.Linq.Queries
{
    public class GetNotionsTypesQuery<TResulItem> :
        LinqQueryBase<IReadOnlyCollection<TResulItem>, GetNotionTypesSpecification<TResulItem>>,
        IGetNotionTypesQuery<TResulItem>
    {
        public GetNotionsTypesQuery([NotNull] ILinqProvider lingProvider)
            : base(lingProvider)
        {
        }

        public override IReadOnlyCollection<TResulItem> Execute(GetNotionTypesSpecification<TResulItem> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return LingProvider.Query<NotionType>().Select(specification.Projection).ToList();
        }
    }
}