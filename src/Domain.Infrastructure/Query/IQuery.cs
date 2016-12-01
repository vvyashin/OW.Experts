using JetBrains.Annotations;

namespace Domain.Infrastructure
{
    public interface IQuery<out TResult, in TSpecification>
    {
        [NotNull]
        TResult Execute([NotNull] TSpecification specification);
    }
}
