using System;
using JetBrains.Annotations;

namespace Domain.Infrastructure
{
    public abstract class LinqQueryBase<TResult, TSpecification> : IQuery<TResult, TSpecification>
    {
        [NotNull]
        protected readonly ILinqProvider LingProvider;

        protected LinqQueryBase([NotNull] ILinqProvider lingProvider)
        {
            if (lingProvider == null) throw new ArgumentNullException(nameof(lingProvider));
            LingProvider = lingProvider;
        }

        public abstract TResult Execute(TSpecification specification);
    }
}
