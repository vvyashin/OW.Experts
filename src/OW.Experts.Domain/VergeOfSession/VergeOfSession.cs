using System;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain
{
    public class VergeOfSession : DomainObject
    {
        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        public VergeOfSession() { }

        public VergeOfSession([NotNull] Verge verge, [NotNull] SessionOfExperts sessionOfExperts, int weight)
        {
            if (verge == null) throw new ArgumentNullException(nameof(verge));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            Verge = verge;
            SessionOfExperts = sessionOfExperts;
            Weight = weight;
        }

        [NotNull]
        public virtual Verge Verge { get; }

        [NotNull]
        public virtual SessionOfExperts SessionOfExperts { get; }

        public virtual int Weight { get; }
    }
}
