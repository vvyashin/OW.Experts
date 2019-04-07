using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo("Domain.Services.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Domain.Services
{
    internal class ExpertService
    {
        private readonly IExpertRepository _expertRepository;
        private readonly ITypeRepository<NotionType> _notionTypeRepository;
        private readonly IRelationTypeRepository _relationTypeRepository;
        private readonly IAssociationRepository _associationRepository;
        private readonly IRelationRepository _relationRepository;

        protected ExpertService() { }
        
        internal ExpertService([NotNull] IExpertRepository expertRepository, 
            [NotNull] ITypeRepository<NotionType> notionTypeRepository,
            [NotNull] IRelationTypeRepository relationTypeRepository,
            [NotNull] IAssociationRepository associationRepository, [NotNull] IRelationRepository relationRepository)
        {
            if (expertRepository == null) throw new ArgumentNullException(nameof(expertRepository));
            if (notionTypeRepository == null) throw new ArgumentNullException(nameof(notionTypeRepository));
            if (relationTypeRepository == null) throw new ArgumentNullException(nameof(relationTypeRepository));
            if (associationRepository == null) throw new ArgumentNullException(nameof(associationRepository));
            if (relationRepository == null) throw new ArgumentNullException(nameof(relationRepository));

            _expertRepository = expertRepository;
            _notionTypeRepository = notionTypeRepository;
            _relationTypeRepository = relationTypeRepository;
            _associationRepository = associationRepository;
            _relationRepository = relationRepository;
        }

        /// <summary>
        /// Checks that expert has joined the session
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <returns>If expert join the current session returns true else returns false</returns>
        /// <remarks>If current session does no exist returns false</remarks>
        public virtual void JoinSession([NotNull] string expertName, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = new Expert(expertName, sessionOfExperts);
            _expertRepository.AddOrUpdate(expert);
        }

        [CanBeNull]
        private Expert GetExpertByNameAndSession([NotNull] string expertName,
            [NotNull] SessionOfExperts sessionOfExperts, ExpertFetch expertFetch)
        {
            var expert = _expertRepository.GetExpertByNameAndSession(
                new GetExpertByNameAndSessionSpecification(expertName, sessionOfExperts, expertFetch));

            return expert;
        }

        private void IfExpertDoesNotExistThrow([CanBeNull] Expert expert,
            [NotNull] string name, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (expert == null) {
                throw new InvalidOperationException($"Expert with name {name} doesn't join session {sessionOfExperts}");
            }
        }

        /// <summary>
        /// Replaces associations of the expert with new
        /// </summary>
        /// <param name="notions">Notions of associations</param>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of experts</param>
        public virtual void Associations([NotNull] IReadOnlyCollection<string> notions, [NotNull] string expertName,
            [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (notions == null) throw new ArgumentNullException(nameof(notions));
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.Associations);
            IfExpertDoesNotExistThrow(expert, expertName, sessionOfExperts);

            // ReSharper disable once PossibleNullReferenceException
            expert.ReplaceAllAssociations(notions);
            
            _expertRepository.AddOrUpdate(expert);
        }

        /// <summary>
        /// Sets types for associations of expert
        /// </summary>
        /// <param name="associations">List of plain objects, that contains id of association and its type</param>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of experts</param>
        public virtual void AssociationsTypes([NotNull] IReadOnlyCollection<AssociationDto> associations, 
            [NotNull] string expertName, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (associations == null) throw new ArgumentNullException(nameof(associations));
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.Associations);
            IfExpertDoesNotExistThrow(expert, expertName, sessionOfExperts);

            foreach (var association in associations) {
                var typeNode = _notionTypeRepository.GetById(association.TypeId);
                // ReSharper disable once PossibleNullReferenceException
                expert.SetTypeForAssociation(association.Id, typeNode, association.OfferType);
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            _expertRepository.AddOrUpdate(expert);
        }

        /// <summary>
        /// Sets types for relations of expert
        /// </summary>
        /// <param name="relationTuple">Plain object, that contains id of relation and flags of existence any relations</param>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of experts</param>
        public virtual void RelationTypes([NotNull] RelationTupleDto relationTuple, [NotNull] string expertName,
            [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (relationTuple == null) throw new ArgumentNullException(nameof(relationTuple));
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.Relations);
            IfExpertDoesNotExistThrow(expert, expertName, sessionOfExperts);

            var straightRelationTypes = new List<RelationType>();
            var reverseRelationTypes = new List<RelationType>();

            if (relationTuple.DoesRelationExist) {
                var generalType = _relationTypeRepository.GetGeneralType();
                var taxonomyType = _relationTypeRepository.GetTaxonomyType();
                var meronomyType = _relationTypeRepository.GetMeronomyType();

                straightRelationTypes.Add(generalType);
                reverseRelationTypes.Add(generalType);

                if (relationTuple.IsStraightTaxonym) {
                    straightRelationTypes.Add(taxonomyType);
                }
                if (relationTuple.IsStraightMeronym) {
                    straightRelationTypes.Add(meronomyType);
                }
                if (relationTuple.IsReverseTaxonym) {
                    reverseRelationTypes.Add(taxonomyType);
                }
                if (relationTuple.IsReverseMeronym) {
                    reverseRelationTypes.Add(meronomyType);
                }
            }

            // ReSharper disable once PossibleNullReferenceException
            expert.SetTypesForRelation(relationTuple.StraightRelationId, straightRelationTypes, null);
            expert.SetTypesForRelation(relationTuple.ReverseRelationId, reverseRelationTypes, null);
            _expertRepository.AddOrUpdate(expert);
        }

        /// <summary>
        /// Checks that expert has joined the session
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of experts</param>
        /// <returns>If expert join the current session returns true else returns false</returns>
        /// <remarks>If current session does no exist returns false</remarks>
        public virtual bool DoesExpertJoinSession([NotNull] string expertName, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.None);
            return expert != null;
        }

        /// <summary>
        /// Check that expert completed current phase
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of experts</param>
        /// <returns>if expert completed current phase returns true else returns false</returns>
        public virtual bool DoesExpertCompleteCurrentPhase([NotNull] string expertName,
            [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.None);
            return expert != null && expert.IsPhaseCompleted;
        }

        /// <summary>
        /// Get all associations of the expert
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of expers</param>
        /// <returns>Association collection</returns>
        [NotNull]
        public virtual IReadOnlyCollection<Association> GetAssociationsByExpertNameAndSession(
            [NotNull] string expertName, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.Associations);
            IfExpertDoesNotExistThrow(expert, expertName, sessionOfExperts);

            // ReSharper disable once PossibleNullReferenceException
            return expert.Associations;
        }

        /// <summary>
        /// Gets next relation pair that had not chosen
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <param name="sessionOfExperts">Session of expert</param>
        /// <returns>Relation pair</returns>
        public virtual Tuple<Relation, Relation> GetNextRelationPairByExpertNameAndSession([NotNull] string expertName, 
            [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.Relations);
            IfExpertDoesNotExistThrow(expert, expertName, sessionOfExperts); // check expert null

            // ReSharper disable once PossibleNullReferenceException
            return expert.GetNextRelationPair();
        }

        /// <summary>
        /// Generate new relations for each expert of the session
        /// </summary>
        /// <param name="sessionOfExperts">Session of experts</param>
        /// <param name="nodesOfSession">Nodes of the session</param>
        public virtual void CreateRelations(SessionOfExperts sessionOfExperts, IReadOnlyCollection<Node> nodesOfSession)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));
            if (nodesOfSession == null) throw new ArgumentNullException(nameof(nodesOfSession));

            var experts =
                _expertRepository.GetExpertsBySession(new GetExpertsBySessionSpecification(sessionOfExperts,
                    ExpertFetch.Relations));
            var nodes = nodesOfSession.Where(n => n.Notion != sessionOfExperts.BaseNotion).ToArray();
            if (experts != null) {
                foreach (var expert in experts) {
                    expert.GenerateRelations(nodes);
                    _expertRepository.AddOrUpdate(expert);
                }
            }
        }

        /// <summary>
        /// Gets all associations grouped by notion and type
        /// </summary>
        /// <param name="sessionOfExperts">Session of experts</param>
        /// <returns>Collection of NodeCandidate</returns>
        [NotNull]
        public virtual IReadOnlyCollection<NodeCandidate> GetNodeCandidatesBySession(
            [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            return _associationRepository.GetNodeCandidatesBySession(sessionOfExperts);
        }

        /// <summary>
        /// Get grouped by source and destination nodes and type
        /// </summary>
        /// <param name="sessionOfExperts">Session of experts</param>
        /// <returns></returns>
        [NotNull]
        public virtual IReadOnlyCollection<GroupedRelation> GetGroupedRelations([NotNull] SessionOfExperts sessionOfExperts)
        {
            return _relationRepository.GetGroupedRelations(sessionOfExperts);
        }

        public virtual void FinishCurrentPhase([NotNull] string expertName, [NotNull] SessionOfExperts sessionOfExperts)
        {
            var expert = GetExpertByNameAndSession(expertName, sessionOfExperts, ExpertFetch.None);
            IfExpertDoesNotExistThrow(expert, expertName, sessionOfExperts);

            // ReSharper disable once PossibleNullReferenceException
            expert.FinishCurrentPhase();
            _expertRepository.AddOrUpdate(expert);
        }
    }
}
