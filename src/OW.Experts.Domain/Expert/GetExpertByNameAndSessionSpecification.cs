using System;
using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public class GetExpertByNameAndSessionSpecification
    {
        public GetExpertByNameAndSessionSpecification(
            [NotNull] string expertName,
            [NotNull] SessionOfExperts sessionOfExperts,
            ExpertFetch fetch)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            ExpertName = expertName;
            SessionOfExperts = sessionOfExperts;
            Fetch = fetch;
        }

        /// <summary>
        /// Gets expert login name.
        /// </summary>
        public string ExpertName { get; }

        /// <summary>
        /// Gets the session where the expert participates.
        /// </summary>
        public SessionOfExperts SessionOfExperts { get; }

        /// <summary>
        /// Gets the strategy of the data fetching.
        /// </summary>
        public ExpertFetch Fetch { get; }
    }
}