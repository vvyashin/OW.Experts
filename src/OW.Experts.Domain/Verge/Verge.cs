using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.Maintainability",
        "SA1401:FieldsMustBePrivate",
        Justification = "Domain Object have protected collections with underscoreCamelCase naming for ORM mapping")]
    public class Verge : DomainObject
    {
        #region Rules

        public static readonly string GeneralTypeName = "Ассоциация";

        #endregion

        // ReSharper disable once InconsistentNaming
        protected IList<VergeOfSession> _sessionWeightSlices;

        public Verge([NotNull] Node sourceNode, [NotNull] Node destinationNode, [NotNull] RelationType type, int weight)
        {
            if (sourceNode == null) throw new ArgumentNullException(nameof(sourceNode));
            if (destinationNode == null) throw new ArgumentNullException(nameof(destinationNode));
            if (type == null) throw new ArgumentNullException(nameof(type));

            SourceNode = sourceNode;
            DestinationNode = destinationNode;
            Type = type;
            Weight = weight;
            _sessionWeightSlices = new List<VergeOfSession>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Verge"/> class.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Verge()
        {
        }

        [NotNull]
        public virtual Node SourceNode { get; }

        [NotNull]
        public virtual Node DestinationNode { get; }

        [NotNull]
        public virtual RelationType Type { get; }

        public virtual int Weight { get; }

        [NotNull]
        public virtual IReadOnlyCollection<VergeOfSession> SessionWeightSlices
            => new ReadOnlyCollection<VergeOfSession>(_sessionWeightSlices);

        [NotNull]
        public virtual Verge UpdateWeightFromSession(int addedWeight, SessionOfExperts sessionOfExperts)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));
            if (addedWeight < 0) throw new ArgumentException("Weight should not be negative");

            _sessionWeightSlices.Add(new VergeOfSession(this, sessionOfExperts, addedWeight));

            var newWeight = (Weight + addedWeight) / 2;

            return new Verge(SourceNode, DestinationNode, Type, newWeight);
        }
    }
}