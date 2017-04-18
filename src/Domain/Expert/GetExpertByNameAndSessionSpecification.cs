using System;
using JetBrains.Annotations;

namespace Domain
{
    public class GetExpertByNameAndSessionSpecification
    {
        public GetExpertByNameAndSessionSpecification([NotNull] string expertName,
            [NotNull] SessionOfExperts sessionOfExperts, ExpertFetch fetch)
        {
            if (expertName == null)
                throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            ExpertName = expertName;
            SessionOfExperts = sessionOfExperts;
            Fetch = fetch;
        }

        public string ExpertName { get; }

        public SessionOfExperts SessionOfExperts { get; }

        /// <summary>
        /// Fetch strategy
        /// </summary>
        public ExpertFetch Fetch{ get; }
    }
}
