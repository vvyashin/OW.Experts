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
    public class Node : DomainObject
    {
        // ReSharper disable once InconsistentNaming
        protected readonly IList<Verge> _ingoingVerges;

        // ReSharper disable once InconsistentNaming
        protected readonly IList<Verge> _outgoingVerges;

        // ReSharper disable once InconsistentNaming
        protected readonly IList<SessionOfExperts> _sessionsOfExperts;

        public Node([NotNull] string notion, [NotNull] NotionType type)
        {
            if (string.IsNullOrWhiteSpace(notion))
                throw new ArgumentException("Notion should not be empty", nameof(notion));
            if (type == null) throw new ArgumentNullException(nameof(type));

            Notion = notion;
            Type = type;
            _sessionsOfExperts = new List<SessionOfExperts>();
            _ingoingVerges = new List<Verge>();
            _outgoingVerges = new List<Verge>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <remarks>Empty ctor can be used only by ORM.</remarks>
        protected Node()
        {
        }

        [NotNull]
        public virtual string Notion { get; }

        [NotNull]
        public virtual NotionType Type { get; }

        [NotNull]
        public virtual IReadOnlyCollection<SessionOfExperts> SessionsOfExperts
            => new ReadOnlyCollection<SessionOfExperts>(_sessionsOfExperts);

        [NotNull]
        public virtual IReadOnlyCollection<Verge> IngoingVerges
            => new ReadOnlyCollection<Verge>(_ingoingVerges);

        [NotNull]
        public virtual IReadOnlyCollection<Verge> OutgoingVerges
            => new ReadOnlyCollection<Verge>(_outgoingVerges);

        public virtual void AddSessionOfExperts([NotNull] SessionOfExperts sessionOfExperts)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            _sessionsOfExperts.Add(sessionOfExperts);
        }

        public override int GetHashCode()
        {
            return Notion.GetHashCode() ^ Type.GetHashCode();
        }

        protected override bool Equals(DomainObject obj)
        {
            if (!(obj is Node)) return false;

            var node = (Node)obj;
            return StringComparer.OrdinalIgnoreCase.Equals(Notion, node.Notion) && Type == node.Type;
        }
    }
}