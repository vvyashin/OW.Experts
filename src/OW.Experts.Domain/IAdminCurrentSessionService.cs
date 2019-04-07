using System.Collections.Generic;
using JetBrains.Annotations;

namespace Domain
{
    public interface IAdminCurrentSessionService : ICurrentSessionService
    {
        void StartNewSession([NotNull] string baseNotion);

        void NextPhase();

        [NotNull]
        IReadOnlyCollection<NodeCandidate> GetAllNodeCandidates();

        int GetExpertCount();

        void CreateSemanticNetworkFromNodeCandidates([NotNull] IReadOnlyCollection<NodeCandidate> nodeCandidates);
        
        void SaveRelationsAsVergesOfSemanticNetwork();

        SemanticNetworkReadModel GetSemanticNetwork();
    }
}
