using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain.Linq
{
    public class GetNotionsTypesQuery<TResulItem> : LinqQueryBase<IReadOnlyCollection<TResulItem>, GetNotionTypesSpecification<TResulItem>>, 
        IGetNotionTypesQuery<TResulItem>
    {
        public GetNotionsTypesQuery([NotNull] ILinqProvider lingProvider) : base(lingProvider)
        {
        }

        public override IReadOnlyCollection<TResulItem> Execute(GetNotionTypesSpecification<TResulItem> specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return LingProvider.Query<NotionType>().Select(specification.Projection).ToList();
        }
    }
}
