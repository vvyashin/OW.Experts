using System;
using JetBrains.Annotations;

namespace Domain
{
    public class GetExpertsBySessionSpecification
    {
        public GetExpertsBySessionSpecification([NotNull] SessionOfExperts sessionOfExperts, ExpertFetch fetch)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            SessionOfExperts = sessionOfExperts;
            Fetch = fetch;
        }

        public SessionOfExperts SessionOfExperts { get; }

        /// <summary>
        /// Fetch strategy
        /// </summary>
        public ExpertFetch Fetch { get; }
    }
}
