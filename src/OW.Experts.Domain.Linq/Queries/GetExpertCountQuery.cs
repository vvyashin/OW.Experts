using System;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;

namespace OW.Experts.Domain.Linq.Queries
{
    internal class GetExpertCountQuery : LinqQueryBase<int, GetExpertCountSpecification>
    {
        public GetExpertCountQuery([NotNull] ILinqProvider lingProvider) : base(lingProvider)
        {
        }

        public override int Execute([NotNull] GetExpertCountSpecification specification)
        {
            if (specification == null) throw new ArgumentNullException(nameof(specification));

            return LingProvider.Query<Expert>().Count(x => x.SessionOfExperts == specification.SessionOfExperts);
        }
    }
}
