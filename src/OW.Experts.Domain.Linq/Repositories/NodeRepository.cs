using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure.Fetching;
using OW.Experts.Domain.Infrastructure.Query;
using OW.Experts.Domain.Infrastructure.Repository;

namespace OW.Experts.Domain.Linq.Repositories
{
    public class NodeRepository : INodeRepository
    {
        private readonly ILinqProvider _linqProvider;
        private readonly IRepository<Node> _repository;

        public NodeRepository([NotNull] IRepository<Node> repository, [NotNull] ILinqProvider linqProvider)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (linqProvider == null) throw new ArgumentNullException(nameof(linqProvider));

            _repository = repository;
            _linqProvider = linqProvider;
        }

        public void AddOrUpdate(Node entity)
        {
            _repository.AddOrUpdate(entity);
        }

        public Node GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public void Remove(Node entity)
        {
            _repository.Remove(entity);
        }

        public Node GetByNotionAndType(string notion, NotionType type)
        {
            return _linqProvider.Query<Node>().SingleOrDefault(x => x.Notion == notion && x.Type == type);
        }

        public IReadOnlyCollection<Node> GetBySession([NotNull] SessionOfExperts session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            return _linqProvider.Query<Node>().Where(x => x.SessionsOfExperts.Contains(session)).ToList();
        }

        public SemanticNetworkReadModel GetSemanticNetworkBySession([NotNull] SessionOfExperts session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            Func<VergeOfSession, VergeReadModel> vergeProjection = sessionVerge =>
                new VergeReadModel(
                    sessionVerge.Verge.SourceNode.Notion,
                    sessionVerge.Verge.SourceNode.Type.Name,
                    sessionVerge.Verge.DestinationNode.Notion,
                    sessionVerge.Verge.DestinationNode.Type.Name,
                    sessionVerge.Verge.Type.Name,
                    sessionVerge.Weight);

            _linqProvider.Query<Verge>()
                .Where(x => x.SourceNode.SessionsOfExperts.Contains(session) ||
                            x.SourceNode.SessionsOfExperts.Contains(session))
                .FetchMany(x => x.SessionWeightSlices).ThenFetch(x => x.SessionOfExperts)
                .Fetch(x => x.SourceNode)
                .Fetch(x => x.DestinationNode)
                .Fetch(x => x.Type)
                .ToFuture();

            _linqProvider.Query<Node>()
                .Where(x => x.SessionsOfExperts.Contains(session))
                .Fetch(x => x.Type)
                .FetchMany(x => x.IngoingVerges)
                .ToFuture();

            var nodes = _linqProvider.Query<Node>()
                .Where(x => x.SessionsOfExperts.Contains(session))
                .FetchMany(x => x.OutgoingVerges)
                .ToFuture()
                .ToList();

            var concepts = nodes
                .Select(x => new ConceptReadModel(
                    x.Notion,
                    x.Type.Name,
                    x.IngoingVerges
                        .SelectMany(v => v.SessionWeightSlices)
                        .Where(sv => sv.SessionOfExperts == session)
                        .Select(vergeProjection)
                        .ToList(),
                    x.OutgoingVerges
                        .SelectMany(v => v.SessionWeightSlices)
                        .Where(sv => sv.SessionOfExperts == session)
                        .Select(vergeProjection).ToList()))
                .ToList();

            return new SemanticNetworkReadModel(concepts);
        }
    }
}