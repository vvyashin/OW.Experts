using System.Collections.Generic;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain
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
