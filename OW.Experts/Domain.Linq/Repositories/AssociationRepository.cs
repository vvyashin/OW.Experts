using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain.Linq
{
    public class AssociationRepository : IAssociationRepository
    {
        private readonly ILinqProvider _linqProvider;

        public AssociationRepository([NotNull] ILinqProvider linqProvider)
        {
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _linqProvider = linqProvider;
        }

        public IReadOnlyCollection<NodeCandidate> GetNodeCandidatesBySession(SessionOfExperts session)
        {
            var totalExpectCount = new GetExpertCountQuery(_linqProvider).Execute(
                new GetExpertCountSpecification(session));

            return _linqProvider.Query<Association>()
                .Where(x => x.Expert.SessionOfExperts == session)
                .GroupBy(x => new {x.Notion, TypeName = x.Type.Name, TypeId = x.Type.Id})
                .Select(gr => new NodeCandidate()
                {
                    Notion = gr.Key.Notion,
                    TypeId = gr.Key.TypeId,
                    TypeName = gr.Key.TypeName,
                    ExpertCount = gr.Count(),
                    TotalExpert = totalExpectCount
                })
            .ToList();
        }
    }
}
