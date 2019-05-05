using System;
using System.Collections.Generic;
using System.Linq;
using OW.Experts.Domain.Infrastructure.Fetching;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.Linq.Repositories
{
    public class ExpertRepository : IExpertRepository
    {
        private readonly ILinqProvider _linqProvider;
        private readonly IRepository<Expert> _repository;

        public ExpertRepository(IRepository<Expert> repository, ILinqProvider linqProvider)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _repository = repository;
            _linqProvider = linqProvider;
        }

        public Expert GetExpertByNameAndSession(GetExpertByNameAndSessionSpecification specification)
        {
            return FetchQuery(specification.Fetch).SingleOrDefault(
                x =>
                    x.Name == specification.ExpertName &&
                    x.SessionOfExperts == specification.SessionOfExperts);
        }

        public IReadOnlyCollection<Expert> GetExpertsBySession(GetExpertsBySessionSpecification specification)
        {
            return FetchQuery(specification.Fetch)
                .Where(x => x.SessionOfExperts == specification.SessionOfExperts)
                .ToList();
        }

        public void AddOrUpdate(Expert entity)
        {
            _repository.AddOrUpdate(entity);
        }

        public Expert GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public void Remove(Expert entity)
        {
            _repository.Remove(entity);
        }

        private IQueryable<Expert> FetchQuery(ExpertFetch expertFetch)
        {
            var query = _linqProvider.Query<Expert>();
            if ((expertFetch & ExpertFetch.Associations) == ExpertFetch.Associations)
                query.FetchMany(x => x.Associations);
            if ((expertFetch & ExpertFetch.Relations) == ExpertFetch.Relations) query.FetchMany(x => x.Relations);

            return query;
        }
    }
}