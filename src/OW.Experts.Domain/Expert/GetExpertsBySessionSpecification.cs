using System;
using JetBrains.Annotations;

namespace OW.Experts.Domain
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
        /// Gets the strategy of the data fetching.
        /// </summary>
        public ExpertFetch Fetch { get; }
    }
}