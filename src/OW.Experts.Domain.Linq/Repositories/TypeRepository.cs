using System;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.Linq.Repositories
{
    public abstract class TypeRepository<T> : ITypeRepository<T>
        where T : TypeBase
    {
        private readonly IRepository<T> _repository;
        protected readonly ILinqProvider LinqProvider;

        protected TypeRepository([NotNull] IRepository<T> repository, [NotNull] ILinqProvider linqProvider)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _repository = repository;
            LinqProvider = linqProvider;
        }

        public T GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public abstract T GetGeneralType();
    }
}
