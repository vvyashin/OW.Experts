using System.Collections.Generic;
using JetBrains.Annotations;

namespace Domain
{
    public interface IAssociationRepository
    {
        /// <summary>
        /// Get associations by the session grouped by notion and type of association
        /// </summary>
        /// <param name="session">Session of experts</param>
        /// <returns>Read only collection of grouped associations</returns>
        [NotNull] 
        IReadOnlyCollection<NodeCandidate> GetNodeCandidatesBySession([NotNull] SessionOfExperts session);
    }
}
