using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain
{
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.Maintainability",
        "SA1401:FieldsMustBePrivate",
        Justification = "Domain Object have protected collections with underscoreCamelCase naming for ORM mapping")]
    public class Expert : DomainObject
    {
        // ReSharper disable once InconsistentNaming
        protected readonly IList<Association> _associations;

        // ReSharper disable once InconsistentNaming
        protected readonly IList<Relation> _relations;

        /// <summary>
        /// Initializes a new instance of the <see cref="Expert"/> class.
        /// </summary>
        /// <param name="name">Login name of the expert.</param>
        /// <param name="sessionOfExperts">Session of experts, in which the expert participates.</param>
        public Expert([NotNull] string name, [NotNull] SessionOfExperts sessionOfExperts)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should not contain only whitespaces", nameof(name));
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            Name = name;
            SessionOfExperts = sessionOfExperts;
            _associations = new List<Association>();
            _relations = new List<Relation>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expert"/> class.
        /// </summary>
        /// <remarks>Empty ctor can be used only by ORM.</remarks>
        // ReSharper disable once UnusedMember.Global
        protected Expert()
        {
            _associations = new List<Association>();
            _relations = new List<Relation>();
        }

        /// <summary>
        /// Gets login name of the expert.
        /// </summary>
        [NotNull]
        public virtual string Name { get; }

        /// <summary>
        /// Gets session of experts, in which the expert participates.
        /// </summary>
        [NotNull]
        public virtual SessionOfExperts SessionOfExperts { get; }

        /// <summary>
        /// Gets associations suggested by the expert.
        /// </summary>
        [NotNull]
        public virtual IReadOnlyCollection<Association> Associations
            => new ReadOnlyCollection<Association>(_associations);

        /// <summary>
        /// Gets relations between associations suggested by the expert.
        /// </summary>
        [NotNull]
        public virtual IReadOnlyCollection<Relation> Relations
            => new ReadOnlyCollection<Relation>(_relations);

        /// <summary>
        /// Gets a value indicating whether the expert complete current phase of the session.
        /// </summary>
        public virtual bool IsPhaseCompleted => LastCompletedPhase == SessionOfExperts.CurrentPhase;

        /// <summary>
        /// Gets or sets last phase, completed by the expert.
        /// </summary>
        public virtual SessionPhase? LastCompletedPhase { get; set; }

        /// <summary>
        /// Clears all associations added earlier and adds new associations.
        /// </summary>
        /// <param name="notions">collection association notions.</param>
        public virtual void ReplaceAllAssociations([NotNull] IReadOnlyCollection<string> notions)
        {
            if (notions == null) throw new ArgumentNullException(nameof(notions));

            _associations.Clear();
            _associations.AddRange(notions.Select(notion => new Association(this, notion)));
        }

        /// <summary>
        /// Sets type to the association found by id.
        /// </summary>
        /// <param name="associationId">Id of the association for updating.</param>
        /// <param name="type">Type from list of known types.</param>
        /// <param name="offerType">New type name suggested by the expert.</param>
        public virtual void SetTypeForAssociation(
            Guid associationId,
            [NotNull] NotionType type,
            [CanBeNull] string offerType)
        {
            if (associationId == Guid.Empty)
                throw new ArgumentException("Id should not be empty", nameof(associationId));
            if (type == null) throw new ArgumentNullException(nameof(type));

            var expertAssociation = Associations.SingleOrDefault(m => m.Id == associationId);
            if (expertAssociation == null) {
                throw new InvalidOperationException(
                    $"It's not possible set types for non existence association {associationId} for expert {this}");
            }

            expertAssociation.UpdateTypes(type, offerType);
        }

        /// <summary>
        /// Generates expert relations from one node to another one for all provided nodes.
        /// </summary>
        /// <param name="nodes">Collection of nodes to create all existence relations between them.</param>
        public virtual void GenerateRelations([NotNull] IReadOnlyCollection<Node> nodes)
        {
            foreach (var firstNode in nodes) {
                foreach (var secondNode in nodes) {
                    if (!ReferenceEquals(firstNode, secondNode)) {
                        _relations.Add(new Relation(this, firstNode, secondNode));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the next unhandled relation pair.
        /// </summary>
        /// <returns>Next unhandled relation pair.</returns>
        [CanBeNull]
        public virtual Tuple<Relation, Relation> GetNextRelationPair()
        {
            var first = Relations.FirstOrDefault(relation => relation.IsChosen == ChosenState.HadNotChosen);
            if (first == null) return null;

            var second = Relations.SingleOrDefault(
                relation => relation.Source.Equals(first.Destination)
                            && relation.Destination.Equals(first.Source));

            if (second == null) {
                throw new Exception(
                    $"{this} has not relation {first.Destination} - {first.Source} " +
                    $"although has {first.Source} - {first.Destination}");
            }

            return new Tuple<Relation, Relation>(first, second);
        }

        /// <summary>
        /// Sets types to the relation found by id.
        /// </summary>
        /// <param name="relationId">Id of the relation for updating.</param>
        /// <param name="types">Types that should be assigned to the relation.</param>
        /// <param name="offerType">Type suggested by the expert that should be assigned to the relation.</param>
        public virtual void SetTypesForRelation(
            Guid relationId,
            [NotNull] IReadOnlyCollection<RelationType> types,
            [CanBeNull] string offerType)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            if (relationId == Guid.Empty)
                throw new ArgumentException("Id should not be empty", nameof(relationId));

            var relationForEdit = Relations.SingleOrDefault(r => r.Id == relationId);
            if (relationForEdit == null) {
                throw new InvalidOperationException(
                    $"It's not possible set types for non existence relation {relationId} by expert {this}");
            }

            relationForEdit.UpdateTypes(types, offerType);
        }

        /// <summary>
        /// Marks current session phase as completed for the expert.
        /// </summary>
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