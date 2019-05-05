using System.Collections.Generic;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain
{
    public interface ISessionOfExpertsRepository : IRepository<SessionOfExperts>
    {
        [NotNull]
        IReadOnlyCollection<SessionOfExperts> GetAllEndedSessionsOfExperts();

        [CanBeNull]
        SessionOfExperts GetCurrent();

        int GetExpertCount([NotNull] SessionOfExperts sessionOfExperts);
    }
}