using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain
{
    public class Expert : DomainObject
    {
        // ReSharper disable once InconsistentNaming
        protected readonly IList<Association> _associations;
        // ReSharper disable once InconsistentNaming
        protected readonly IList<Relation> _relations;

        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Expert()
        {
            _associations = new List<Association>();
            _relations = new List<Relation>();
        }

        /// <summary>
        /// Ctor for creating new object
        /// </summary>
        /// <param name="name">Name (login) of expert</param>
        /// <param name="sessionOfExperts">Session of experts, in which expert participates</param>
        public Expert([NotNull] string name, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should not contains only whitespaces", nameof(name));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            Name = name;
            SessionOfExperts = sessionOfExperts;
            _associations = new List<Association>();
            _relations = new List<Relation>();
        }

        [NotNull]
        public virtual string Name { get; }

        /// <summary>
        /// Session of experts, in which the expert participates
        /// </summary>
        [NotNull]
        public virtual SessionOfExperts SessionOfExperts { get; }

        /// <summary>
        /// Associations offered by the expert
        /// </summary>
        [NotNull]
        public virtual IReadOnlyCollection<Association> Associations
            => new ReadOnlyCollection<Association>(_associations);

        /// <summary>
        /// relation beetwen assoiciations offered by the expert
        /// </summary>
        [NotNull]
        public virtual IReadOnlyCollection<Relation> Relations 
            => new ReadOnlyCollection<Relation>(_relations);

        /// <summary>
        /// Last phase, completed by the expert
        /// </summary>
        public virtual SessionPhase? LastCompletedPhase { get; set; }

        public virtual bool IsPhaseCompleted => LastCompletedPhase == SessionOfExperts.CurrentPhase;

        /// <summary>
        /// Clear all old associations and add new associations
        /// </summary>
        /// <param name="notions">collection of notion of association</param>
        public virtual void ReplaceAllAssociations([NotNull] IReadOnlyCollection<string> notions)
        {
            if (notions == null) throw new ArgumentNullException(nameof(notions));
            
            _associations.Clear();
            _associations.AddRange(notions.Select(notion => new Association(this, notion)));
        }

        /// <summary>
        /// Set type to association, that got by the identificator
        /// </summary>
        /// <param name="associationId">identificator of association for update</param>
        /// <param name="type">Type from listof known types</param>
        /// <param name="offerType">New type offered by expert</param>
        public virtual void SetTypeForAssociation(Guid associationId, [NotNull] NotionType type, [CanBeNull] string offerType)
        {
            if (associationId == Guid.Empty)
                throw new ArgumentException("Id should not be empty", nameof(associationId));
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            var expertAssociation = Associations.SingleOrDefault(m => m.Id == associationId);
            if (expertAssociation == null) {
                throw new InvalidOperationException(
                    $"It's not possible set types for non existens association {associationId} by expert {this}");
            }
            else {
                expertAssociation.UpdateTypes(type, offerType);
            }
        }

        /// <summary>
        /// Generate relations from each nodes to each nodes
        /// </summary>
        public virtual void GenerateRelations([NotNull] IReadOnlyCollection<Node> nodes)
        {
            foreach (var firstNode in nodes) {
                foreach (var secondNode in nodes) {
                    if (!object.ReferenceEquals(firstNode, secondNode)) {
                        _relations.Add(new Relation(this, firstNode, secondNode));
                    }
                }
            }
        }

        /// <summary>
        /// Get next relation pair (first node to second and second node to first) 
        /// with witch user hasn't worked yet
        /// </summary>
        /// <returns>Next relation pair</returns>
        [CanBeNull]
        public virtual Tuple<Relation, Relation> GetNextRelationPair()
        {
            var first = Relations.FirstOrDefault(relation => relation.IsChosen == ChosenState.HadNotChosen);
            if (first == null) return null;

            var second = Relations.SingleOrDefault(relation => relation.Source.Equals(first.Destination)
                                                               && relation.Destination.Equals(first.Source));
            if (second == null) throw new Exception($"{this} has not relation {first.Destination} - {first.Source} " +
                                                    $"although has {first.Source} - {first.Destination}");

            return new Tuple<Relation, Relation>(first, second);
        }

        /// <summary>
        /// Set types to relation, that got by the identificator
        /// </summary>
        /// <param name="relationId"></param>
        /// <param name="types"></param>
        /// <param name="offerType"></param>
        public virtual void SetTypesForRelation(Guid relationId, [NotNull] IReadOnlyCollection<RelationType> types,
            [CanBeNull] string offerType)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (relationId == Guid.Empty)
                throw new ArgumentException("Id should not be empty", nameof(relationId));

            var relationForEdit = Relations.SingleOrDefault(r => r.Id == relationId);
            if (relationForEdit == null) {
                throw new InvalidOperationException(
                    $"It's not possible set types for non existens relation {relationId} by expert {this}");
            }
            else {
                relationForEdit.UpdateTypes(types, offerType);
            }
        }

        public virtual void FinishCurrentPhase()
        {
            LastCompletedPhase = SessionOfExperts.CurrentPhase;
        }

        public override string ToString()
        {
            return $"{Name} - Сессия: {SessionOfExperts}";
        }
    }
}
