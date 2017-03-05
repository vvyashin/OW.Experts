using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Domain.Services.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Domain.Services
{
    internal class SemanticNetworkService
    {
        private readonly INodeRepository _nodeRepository;
        private readonly IVergeRepository _vergeRepository;
        private readonly ITypeRepository<NotionType>_notionTypeRepository;
        private readonly ITypeRepository<RelationType> _relationTypeRepository;

        protected SemanticNetworkService() { }

        public SemanticNetworkService([NotNull] INodeRepository nodeRepository,
            [NotNull] IVergeRepository vergeRepository,
            [NotNull] ITypeRepository<NotionType> notionTypeRepository,
            [NotNull] ITypeRepository<RelationType> relationTypeRepository)
        {
            if (nodeRepository == null) throw new ArgumentNullException(nameof(nodeRepository));
            if (vergeRepository == null) throw new ArgumentNullException(nameof(vergeRepository));
            if (notionTypeRepository == null) throw new ArgumentNullException(nameof(notionTypeRepository));
            if (relationTypeRepository == null) throw new ArgumentNullException(nameof(relationTypeRepository));
            
            _nodeRepository = nodeRepository;
            _vergeRepository = vergeRepository;
            _notionTypeRepository = notionTypeRepository;
            _relationTypeRepository = relationTypeRepository;
        }

        public virtual void CreateSemanticNetworkFromNodeCandidates(
            [NotNull] IReadOnlyCollection<NodeCandidate> nodeCandidates,
            [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (nodeCandidates == null) throw new ArgumentNullException(nameof(nodeCandidates));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var generalVergeType = _relationTypeRepository.GetGeneralType();
            var generalNodeType = _notionTypeRepository.GetGeneralType();

            var root = GetOrCreateNode(sessionOfExperts.BaseNotion, generalNodeType, sessionOfExperts);
            _nodeRepository.AddOrUpdate(root);

            foreach (var nodeCandidate in nodeCandidates.Where(x => x.IsSaveAsNode)) {
                var node = GetOrCreateNode(nodeCandidate.Notion, _notionTypeRepository.GetById(nodeCandidate.TypeId),
                    sessionOfExperts);
                _nodeRepository.AddOrUpdate(node);

                var straightVerge = UpdateOrCreateVerge(root, node, generalVergeType, nodeCandidate.ExpertPercent, sessionOfExperts);
                var reverseVerge = UpdateOrCreateVerge(node, root, generalVergeType, nodeCandidate.ExpertPercent, sessionOfExperts);

                _vergeRepository.AddOrUpdate(straightVerge);
                _vergeRepository.AddOrUpdate(reverseVerge);
            }
        }

        [NotNull]
        private Node GetOrCreateNode([NotNull] string notion, [NotNull] NotionType type, [NotNull] SessionOfExperts sessionOfExperts)
        {
            var node = _nodeRepository.GetByNotionAndType(notion, type) ?? new Node(notion, type);
            node.AddSessionOfExperts(sessionOfExperts);

            return node;
        }

        [NotNull]
        private Verge UpdateOrCreateVerge([NotNull] Node sourceNode, [NotNull] Node destinationNode, 
            [NotNull] RelationType type, double percent, [NotNull] SessionOfExperts sessionOfExperts)
        {
            var weight = PercentToWeght(percent);
            var verge = _vergeRepository.GetByNodesAndTypes(sourceNode, destinationNode, type) ??
                        new Verge(sourceNode, destinationNode, type, weight);

            verge.UpdateWeightFromSession(weight, sessionOfExperts);
            
            return verge;
        }

        private int PercentToWeght(double percent)
        {
            return (int)Math.Round(percent * 100);
        }

        public virtual void SaveRelationsAsVergesOfSemanticNetwork(
            [NotNull] IReadOnlyCollection<GroupedRelation> groupedRelations,
            [NotNull] SessionOfExperts session)
        {
            if (groupedRelations == null) throw new ArgumentNullException(nameof(groupedRelations));
            if (session == null) throw new ArgumentNullException(nameof(session));

            foreach (var groupedRelation in groupedRelations) {
                var verge = UpdateOrCreateVerge(groupedRelation.Source, groupedRelation.Destination,
                    groupedRelation.Type, groupedRelation.Percent, session);

                _vergeRepository.AddOrUpdate(verge);
            }
        }

        [NotNull]
        public virtual IReadOnlyCollection<Node> GetNodesBySession(SessionOfExperts session)
        {
            return _nodeRepository.GetBySession(session);
        }

        [NotNull]
        public virtual SemanticNetworkReadModel GetSemanticNetworkBySession(SessionOfExperts session)
        {
            return _nodeRepository.GetSemanticNetworkBySession(session);
        }
    }
}
