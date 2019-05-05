using System;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    public class Association : DomainObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Association" /> class.
        /// </summary>
        /// <param name="expert">Expert, who suggested association.</param>
        /// <param name="notion">Association notion.</param>
        public Association([NotNull] Expert expert, [NotNull] string notion)
        {
            if (expert == null) throw new ArgumentNullException(nameof(expert));
            if (string.IsNullOrWhiteSpace(notion))
                throw new ArgumentException("Notion should not be empty string", nameof(notion));

            Expert = expert;
            Notion = notion;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Association" /> class.
        /// </summary>
        /// <remarks>Empty ctor can be used only by ORM.</remarks>
        protected Association()
        {
        }

        /// <summary>
        /// Gets expert who suggested the association.
        /// </summary>
        [NotNull]
        public virtual Expert Expert { get; }

        /// <summary>
        /// Gets association notion.
        /// </summary>
        [NotNull]
        public virtual string Notion { get; }

        /// <summary>
        /// Gets or sets type chosen by the expert from the list of known types.
        /// </summary>
        [CanBeNull]
        public virtual NotionType Type { get; protected set; }

        /// <summary>
        /// Gets or sets type suggested by the expert to add to the list of known types.
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