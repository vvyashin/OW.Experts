using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain
{
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.Maintainability",
        "SA1401:FieldsMustBePrivate",
        Justification = "Domain Object have protected collections with underscoreCamelCase naming for ORM mapping")]
    public class Relation : DomainObject
    {
        // ReSharper disable once InconsistentNaming
        [NotNull]
        protected readonly IList<RelationType> _types;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Relation"/> class.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Relation()
        {
        }

        [NotNull]
        public virtual Expert Expert { get; }

        [NotNull]
        public virtual Node Source { get; }

        [NotNull]
        public virtual Node Destination { get; }

        /// <summary>
        /// Gets types were chosen by expert.
        /// </summary>
        [NotNull]
        public virtual IReadOnlyCollection<RelationType> Types => new ReadOnlyCollection<RelationType>(_types);

        /// <summary>
        /// Gets or sets new types suggested by the expert.
        /// </summary>
        [CanBeNull]
        public virtual string OfferType { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether expert have already made a choice about relation types or not.
        /// </summary>
        public virtual ChosenState IsChosen { get; protected set; }

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