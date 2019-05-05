using System;
using JetBrains.Annotations;

namespace OW.Experts.Domain
{
    public interface ITypeRepository<out T>
        where T : TypeBase
    {
        [NotNull]
        T GetById(Guid id);

        [NotNull]
        T GetGeneralType();
    }
}