using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public interface ICurrentSessionService
    {
        [CanBeNull]
        SessionOfExperts CurrentSession { get; }

        bool DoesCurrentSessionExist { get; }
    }
}