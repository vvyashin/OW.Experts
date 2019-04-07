using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    /// <summary>
    /// Class representing node of semantic network
    /// </summary>
    public class Node : DomainObject
    {
        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Node()
        {
        }

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

        [NotNull]
        public virtual string Notion { get; }

        [NotNull]
        public virtual NotionType Type { get; }

        // ReSharper disable once InconsistentNaming
        protected readonly IList<SessionOfExperts> _sessionsOfExperts;
        [NotNull]
        public virtual IReadOnlyCollection<SessionOfExperts> SessionsOfExperts 
            => new ReadOnlyCollection<SessionOfExperts>(_sessionsOfExperts);

        // ReSharper disable once InconsistentNaming
        protected readonly IList<Verge> _ingoingVerges;
        [NotNull]
        public virtual IReadOnlyCollection<Verge> IngoingVerges 
            => new ReadOnlyCollection<Verge>(_ingoingVerges);

        // ReSharper disable once InconsistentNaming
        protected readonly IList<Verge> _outgoingVerges;
        [NotNull]
        public virtual IReadOnlyCollection<Verge> OutgoingVerges
            => new ReadOnlyCollection<Verge>(_outgoingVerges);

        public virtual void AddSessionOfExperts([NotNull] SessionOfExperts sessionOfExperts)
        {
            if (sessionOfExperts == null) throw new ArgumentNullException(nameof(sessionOfExperts));

            _sessionsOfExperts.Add(sessionOfExperts);
        }

        protected override bool Equals(DomainObject obj)
        {
            if (!(obj is Node)) return false;

            var node = (Node) obj;
            return StringComparer.OrdinalIgnoreCase.Equals(this.Notion, node.Notion) && this.Type == node.Type;
        }

        public override int GetHashCode()
        {
            return Notion.GetHashCode() ^ Type.GetHashCode();
        }
    }
}