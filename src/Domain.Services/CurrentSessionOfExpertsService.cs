using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Domain.Services
{
    public class CurrentSessionOfExpertsService : IAdminCurrentSessionService, IExpertCurrentSessionService
    {
        #region dependencies

        private readonly ISessionOfExpertsRepository _sessionOfExpertsRepository;
        private readonly ExpertService _expertService;
        private readonly SemanticNetworkService _semanticNetworkService;
        
        #endregion

        private const string SessionExistErrorMessage = "Текущая сессия уже существует";
        private const string SessionDoesNotExistErrorMessage = "Текущей сессии еще не существует";
        private const string SessionIsNotInPhase = "На данном этапе тестирования недоступно";

        internal CurrentSessionOfExpertsService([NotNull] ISessionOfExpertsRepository sessionOfExpertsRepository,
            [NotNull] ExpertService expertService, [NotNull] SemanticNetworkService semanticNetworkService)
        {
            _sessionOfExpertsRepository = sessionOfExpertsRepository;
            _expertService = expertService;
            _semanticNetworkService = semanticNetworkService;
        }

        [CanBeNull]
        public SessionOfExperts CurrentSession =>
            _currentSession ?? ( _currentSession = GetCurrentSessionFromRepository() );
        private SessionOfExperts _currentSession;

        public bool DoesCurrentSessionExist => CurrentSession != null;

        [CanBeNull]
        private SessionOfExperts GetCurrentSessionFromRepository()
        {
            return _sessionOfExpertsRepository.GetCurrent();
        }

        private void IfCurrentSessionExistsThrow()
        {
            if (DoesCurrentSessionExist)
                throw new InvalidOperationException(SessionExistErrorMessage);
        }

        private void IfCurrentSessionDoesNotExistThrow()
        {
            if (!DoesCurrentSessionExist)
                throw new InvalidOperationException(SessionDoesNotExistErrorMessage);
        }

        private void IfCurrentSessionIsNotInPhaseThrow(SessionPhase phase)
        {
            IfCurrentSessionDoesNotExistThrow();
            // ReSharper disable once PossibleNullReferenceException
            if (CurrentSession.CurrentPhase != phase)
                throw new InvalidOperationException(SessionIsNotInPhase);
        }

        /// <summary>
        /// Starts new current session
        /// </summary>
        /// <param name="baseNotion"></param>
        public void StartNewSession([NotNull] string baseNotion)
        {
            if (baseNotion == null) throw new ArgumentNullException(nameof(baseNotion));
            IfCurrentSessionExistsThrow();

            var session = new SessionOfExperts(baseNotion);
            _sessionOfExpertsRepository.AddOrUpdate(session);
        }

        /// <summary>
        /// Moves the current session to the next phase
        /// </summary>
        public void NextPhase()
        {
            IfCurrentSessionDoesNotExistThrow();

            var session = CurrentSession;
            // ReSharper disable once PossibleNullReferenceException
            session.NextPhaseOrFinish();
            _sessionOfExpertsRepository.AddOrUpdate(session);

            if (session.CurrentPhase == SessionPhase.SelectingAndSpecifyingRelations) {
                _expertService.CreateRelations(session,
                    _semanticNetworkService.GetNodesBySession(session));
            }
        }

        #region expert facade 

        /// <summary>
        /// Checks that expert has joined the session
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <returns>If expert join the current session returns true else returns false</returns>
        /// <remarks>If current session does no exist returns false</remarks>
        public bool DoesExpertJoinSession([NotNull] string expertName)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));

            return CurrentSession != null && _expertService.DoesExpertJoinSession(expertName, CurrentSession);
        }

        /// <summary>
        /// Joins the expert to the session
        /// </summary>
        /// <param name="expertName">Expert name</param>
        public void JoinSession([NotNull] string expertName)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            IfCurrentSessionIsNotInPhaseThrow(SessionPhase.MakingAssociations);

            // ReSharper disable once AssignNullToNotNullAttribute
            _expertService.JoinSession(expertName, CurrentSession);
        }

        /// <summary>
        /// Adds association to the expert
        /// </summary>
        /// <param name="notions">Notions of associations</param>
        /// <param name="expertName">Expert name</param>
        public void Associations([NotNull] IReadOnlyCollection<string> notions, [NotNull] string expertName)
        {
            if (notions == null) throw new ArgumentNullException(nameof(notions));
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));

            IfCurrentSessionIsNotInPhaseThrow(SessionPhase.MakingAssociations);

            // ReSharper disable once AssignNullToNotNullAttribute
            _expertService.Associations(notions, expertName, CurrentSession);
        }

        /// <summary>
        /// Sets types for associations of the expert
        /// </summary>
        /// <param name="associations">Associations</param>
        /// <param name="expertName">Expert name</param>
        public void AssociationsTypes([NotNull] IReadOnlyCollection<AssociationDto> associations, 
            [NotNull] string expertName)
        {
            if (associations == null) throw new ArgumentNullException(nameof(associations));
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));

            IfCurrentSessionIsNotInPhaseThrow(SessionPhase.SpecifyingAssociationsTypes);

            // ReSharper disable once AssignNullToNotNullAttribute
            _expertService.AssociationsTypes(associations, expertName, CurrentSession);
        }

        /// <summary>
        /// Sets types for generated relations of the expert
        /// </summary>
        /// <param name="relationTupleDto">Pair of relations of expert for setting</param>
        /// <param name="expertName">Expert name</param>
        public void Relations([NotNull] RelationTupleDto relationTupleDto, [NotNull] string expertName)
        {
            if (relationTupleDto == null) throw new ArgumentNullException(nameof(relationTupleDto));
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));

            IfCurrentSessionIsNotInPhaseThrow(SessionPhase.SelectingAndSpecifyingRelations);

            // ReSharper disable once AssignNullToNotNullAttribute
            _expertService.RelationTypes(relationTupleDto, expertName, CurrentSession);
        }

        /// <summary>
        /// Gets all associations grouped by notion and type
        /// </summary>
        /// <returns>Collection of NodeCandidate</returns>
        [NotNull]
        public IReadOnlyCollection<NodeCandidate> GetAllNodeCandidates()
        {
            IfCurrentSessionDoesNotExistThrow();

            // ReSharper disable once AssignNullToNotNullAttribute
            return _expertService.GetNodeCandidatesBySession(CurrentSession);
        }

        /// <summary>
        /// Expert count has joined the session
        /// </summary>
        /// <returns>Expert count</returns>
        public int GetExpertCount()
        {
            IfCurrentSessionDoesNotExistThrow();

            // ReSharper disable once AssignNullToNotNullAttribute
            return _sessionOfExpertsRepository.GetExpertCount(CurrentSession);
        }

        /// <summary>
        /// Check that expert completed current phase of the session
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <returns>if expert completed current phase returns true else returns false</returns>
        public bool DoesExpertCompleteCurrentPhase([NotNull] string expertName)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));

            return CurrentSession != null && _expertService.DoesExpertCompleteCurrentPhase(expertName, CurrentSession);
        }

        /// <summary>
        /// Get all associations of the expert
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <returns>Association collection</returns>
        public IReadOnlyCollection<Association> GetAssociationsByExpertName([NotNull] string expertName)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            IfCurrentSessionDoesNotExistThrow();

            // ReSharper disable once AssignNullToNotNullAttribute
            return _expertService.GetAssociationsByExpertNameAndSession(expertName, CurrentSession);
        }

        /// <summary>
        /// Gets next relation pair that had not chosen
        /// </summary>
        /// <param name="expertName">Expert name</param>
        /// <returns>Relation pair</returns>
        public Tuple<Relation, Relation> GetNextRelationByExpertName([NotNull] string expertName)
        {
            if (expertName == null) throw new ArgumentNullException(nameof(expertName));
            IfCurrentSessionDoesNotExistThrow();

            // ReSharper disable once AssignNullToNotNullAttribute
            return _expertService.GetNextRelationPairByExpertNameAndSession(expertName, CurrentSession);
        }
        #endregion

        #region semantic network facade

        /// <summary>
        /// Creates semantic network from chosen nodes
        /// </summary>
        /// <param name="nodeCandidates">All node candidates</param>
        public void CreateSemanticNetworkFromNodeCandidates([NotNull] IReadOnlyCollection<NodeCandidate> nodeCandidates)
        {
            if (nodeCandidates == null) throw new ArgumentNullException(nameof(nodeCandidates));
            IfCurrentSessionIsNotInPhaseThrow(SessionPhase.SelectingNodes);

            // ReSharper disable once AssignNullToNotNullAttribute
            _semanticNetworkService.CreateSemanticNetworkFromNodeCandidates(nodeCandidates, CurrentSession);
        }

        /// <summary>
        /// Saves relations to semantic network
        /// </summary>
        public void SaveRelationsAsVergesOfSemanticNetwork()
        {
            IfCurrentSessionIsNotInPhaseThrow(SessionPhase.SelectingAndSpecifyingRelations);

            var session = CurrentSession;
            // ReSharper disable once AssignNullToNotNullAttribute
            var groupedRelations = _expertService.GetGroupedRelations(session);
            // ReSharper disable once AssignNullToNotNullAttribute
            _semanticNetworkService.SaveRelationsAsVergesOfSemanticNetwork(groupedRelations, session);
        }

        public SemanticNetworkReadModel GetSemanticNetwork()
        {
            IfCurrentSessionDoesNotExistThrow();

            var session = CurrentSession;

            return _semanticNetworkService.GetSemanticNetworkBySession(session);
        }

        #endregion
    }
}
