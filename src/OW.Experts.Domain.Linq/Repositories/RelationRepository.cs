using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Linq.Queries;

namespace OW.Experts.Domain.Linq.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private readonly ILinqProvider _linqProvider;

        public RelationRepository([NotNull] ILinqProvider linqProvider)
        {
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _linqProvider = linqProvider;
        }

        public IReadOnlyCollection<GroupedRelation> GetGroupedRelations(SessionOfExperts sessionOfExperts)
        {
            var totalExpectCount = new GetExpertCountQuery(_linqProvider).Execute(
                new GetExpertCountSpecification(sessionOfExperts));

            var relations = _linqProvider.Query<Relation>()
                .SelectMany(x => x.Types, (relation, type) => new { relation.Source, relation.Destination, Type = type })
                .ToList();

            return relations
                .GroupBy(x => new { x.Source, x.Destination, x.Type })
                .Select(gr => new GroupedRelation
                {
                    Source = gr.Key.Source,
                    Destination = gr.Key.Destination,
                    Type = gr.Key.Type,
                    ExpertCount = gr.Count(),
                    TotalExpectCount = totalExpectCount
                })
                .ToList();
        }
    }
}