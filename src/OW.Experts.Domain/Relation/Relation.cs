using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain
{
    /// <summary>
    /// Relation and its type offered by expert during the session
    /// </summary>
    public class Relation : DomainObject
    {
        [NotNull]
        protected readonly IList<RelationType> _types;

        [NotNull]
        public virtual Expert Expert { get; }

        [NotNull]
        public virtual Node Source { get; }

        [NotNull]
        public virtual Node Destination { get; }

        /// <summary>
        /// All types were chosen by expert (expert can choose several types from list of known types)
        /// </summary>
        [NotNull]
        public virtual IReadOnlyCollection<RelationType> Types => new ReadOnlyCollection<RelationType>(_types);

        /// <summary>
        /// Type offered by the expert (It's not from list of known types)
        /// </summary>
        [CanBeNull]
        public virtual string OfferType { get; protected set; }

        /// <summary>
        /// Expert had chosen or had not chosen relation
        /// </summary>
        public virtual ChosenState IsChosen { get; protected set; }

        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Relation()
        {
        }

        public Relation([NotNull] Expert expert, [NotNull] Node source, [NotNull] Node destination)
        {
            if (expert == null) throw new ArgumentNullException(nameof(expert));
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));

            Source = source;
            Destination = destination;
            Expert = expert;
            IsChosen = ChosenState.HadNotChosen;
            _types = new List<RelationType>();
        }

        public virtual void UpdateTypes([NotNull] IReadOnlyCollection<RelationType> types, [CanBeNull] string offerType)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));
            
            _types.Clear();
            _types.AddRange(types);

            OfferType = offerType;
            IsChosen = ChosenState.HadChosen;            
        }
    }
}