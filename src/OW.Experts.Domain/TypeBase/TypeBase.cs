using System;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    public abstract class TypeBase : DomainObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBase"/> class.
        /// </summary>
        /// <param name="name">Type name.</param>
        protected TypeBase([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name should not contains only whitespaces");

            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeBase"/> class.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected TypeBase()
        {
        }

        [NotNull]
        public virtual string Name { get; }
    }
}