using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Infrastructure.Repository;
using OW.Experts.Domain.Linq.Queries;

namespace OW.Experts.Domain.Linq.Repositories
{
    public class SessionOfExpertsRepository : ISessionOfExpertsRepository
    {
        private readonly IRepository<SessionOfExperts> _repository;
        private readonly ILinqProvider _linqProvider;

        public SessionOfExpertsRepository([NotNull] IRepository<SessionOfExperts> repository, [NotNull] ILinqProvider linqProvider)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _repository = repository;
            _linqProvider = linqProvider;
        }

        public void AddOrUpdate([NotNull] SessionOfExperts entity)
        {
            _repository.AddOrUpdate(entity);
        }

        [NotNull]
        public SessionOfExperts GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public void Remove([NotNull] SessionOfExperts entity)
        {
            _repository.Remove(entity);
        }

        [NotNull]
        public IReadOnlyCollection<SessionOfExperts> GetAllEndedSessionsOfExperts()
        {
            return _linqProvider.Query<SessionOfExperts>()
                .Where(x => x.CurrentPhase == SessionPhase.Ended)
                .ToList();
        }

        [CanBeNull]
        public SessionOfExperts GetCurrent()
        {
            return _linqProvider
                .Query<SessionOfExperts>()
                .SingleOrDefault(x => x.CurrentPhase != SessionPhase.Ended);
        }

        public int GetExpertCount([NotNull] SessionOfExperts sessionOfExperts)
        {
            return new GetExpertCountQuery(_linqProvider).Execute(
                new GetExpertCountSpecification(sessionOfExperts));
        }
    }
}
