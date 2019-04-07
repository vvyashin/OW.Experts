using JetBrains.Annotations;

namespace OW.Experts.Domain.Infrastructure.Query
{
    public interface IQuery<out TResult, in TSpecification>
    {
        [NotNull]
        TResult Execute([NotNull] TSpecification specification);
    }
}
