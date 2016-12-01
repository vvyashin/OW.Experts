using System;
using JetBrains.Annotations;

namespace Domain.Linq
{
    internal class GetExpertCountSpecification
    {
        public GetExpertCountSpecification([NotNull] SessionOfExperts sessionOfExperts)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            SessionOfExperts = sessionOfExperts;
        }

        public SessionOfExperts SessionOfExperts { get; }
    }
}
