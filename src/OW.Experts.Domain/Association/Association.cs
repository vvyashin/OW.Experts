using System;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    public class Association : DomainObject
    {
        /// <summary>
        /// Ctor only for mapping
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected Association() { }

        /// <summary>
        /// Ctor for creating new object
        /// </summary>
        /// <param name="expert">expert, who offer association</param>
        /// <param name="notion">notion of association</param>
        public Association([NotNull] Expert expert, [NotNull] string notion)
        {
            if (expert == null) throw new ArgumentNullException(nameof(expert));
            if (string.IsNullOrWhiteSpace(notion))
                throw new ArgumentException("Notion should not be empty string", nameof(notion));
            Expert = expert;
            Notion = notion;
        }

        [NotNull]
        public virtual Expert Expert { get; }

        [NotNull]
        public virtual string Notion { get; }

        /// <summary>
        /// Type chosen by the expert from list of known types
        /// </summary>
        [CanBeNull]
        public virtual NotionType Type { get; protected set; }

        /// <summary>
        /// Type offered by the expert (It's not from list of known types).
        /// </summary>
        [CanBeNull]
        public virtual string OfferType { get; protected set; }

        public virtual void UpdateTypes([NotNull] NotionType type, [CanBeNull] string offerType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            Type = type;
            OfferType = offerType;
        }
    }
}
