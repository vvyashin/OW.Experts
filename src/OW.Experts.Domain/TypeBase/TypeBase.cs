using System;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    public abstract class TypeBase : DomainObject
    {
        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected TypeBase() { }


        /// <summary>
        /// Ctor for creating new object
        /// </summary>
        /// <param name="name"></param>
        protected TypeBase([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException(
                "Name should not contains only whitespaces");
            Name = name;
        }

        [NotNull]
        public virtual string Name { get; }
    }
}
