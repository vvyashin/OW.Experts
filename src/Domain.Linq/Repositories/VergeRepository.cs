using System;
using System.Linq;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain.Linq
{
    public class VergeRepository : IVergeRepository
    {
        private readonly IRepository<Verge> _repository;
        private readonly ILinqProvider _linqProvider;

        public VergeRepository([NotNull] IRepository<Verge> repository, [NotNull] ILinqProvider linqProvider)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _repository = repository;
            _linqProvider = linqProvider;
        }
        
        public void AddOrUpdate(Verge entity)
        {
            _repository.AddOrUpdate(entity);
        }

        public Verge GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public void Remove(Verge entity)
        {
            _repository.Remove(entity);
        }

        public Verge GetByNodesAndTypes(Node source, Node destination, RelationType type)
        {
            return _linqProvider.Query<Verge>()
                    .SingleOrDefault(x => x.SourceNode == source && x.DestinationNode == destination && x.Type == type);
        }
    }
}
