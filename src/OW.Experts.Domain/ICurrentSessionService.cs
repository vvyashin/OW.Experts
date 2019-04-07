using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Domain
{
    public interface ICurrentSessionService
    {
        [CanBeNull]
        SessionOfExperts CurrentSession { get; }

        bool DoesCurrentSessionExist { get; }
    }
}
