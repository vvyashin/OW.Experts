using System;
using JetBrains.Annotations;

namespace OW.Experts.Domain.Infrastructure.Query
{
    public abstract class LinqQueryBase<TResult, TSpecification> : IQuery<TResult, TSpecification>
    {
        protected LinqQueryBase([NotNull] ILinqProvider lingProvider)
        {
            if (lingProvider == null) throw new ArgumentNullException(nameof(lingProvider));
            LingProvider = lingProvider;
        }

        [NotNull]
        protected ILinqProvider LingProvider { get; }

        public abstract TResult Execute(TSpecification specification);
    }
}