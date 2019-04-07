using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using OW.Experts.Domain;
using OW.Experts.Domain.Linq.Repositories;

namespace OW.Experts.IntergrationTests.QueriesTests
{
    [TestFixture]
    public class QueriesTests : DropCreateOnOneTimeSetupTestFixture
    {
        public override void OnOneTimeSetup()
        {
            base.OnOneTimeSetup();

            Seed();
        }

        private SessionOfExperts _session1, _session2;
        private Expert _expert1, _expert2, _expert3;

        private void Seed()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                _session1 = new SessionOfExperts("baseNotion");
                _session2 = new SessionOfExperts("otherNotion");

                var sessionRepo = GetRepository<SessionOfExperts>();
                sessionRepo.AddOrUpdate(_session1);
                sessionRepo.AddOrUpdate(_session2);

                _expert1 = new Expert("name1", _session1);
                _expert2 = new Expert("name2", _session2);
                _expert3 = new Expert("name3", _session1);
                var expertRepo = GetRepository<Expert>();
                expertRepo.AddOrUpdate(_expert1);
                expertRepo.AddOrUpdate(_expert2);
                expertRepo.AddOrUpdate(_expert3);

                var notionType = new NotionType("type");
                GetRepository<NotionType>().AddOrUpdate(notionType);

                var nodeRepo = GetRepository<Node>();
                var node1 = new Node("notion1", notionType);
                node1.AddSessionOfExperts(_session1);
                nodeRepo.AddOrUpdate(node1);

                var node2 = new Node("notion2", notionType);
                node2.AddSessionOfExperts(_session1);
                nodeRepo.AddOrUpdate(node2);

                var relationType = new RelationType("type");
                GetRepository<RelationType>().AddOrUpdate(relationType);

                var vergeRepo = GetRepository<Verge>();
                var verge1 = new Verge(node1, node2, relationType, 20);
                verge1.UpdateWeightFromSession(20, _session1);
                vergeRepo.AddOrUpdate(verge1);

                var verge2 = new Verge(node2, node1, relationType, 20);
                verge2.UpdateWeightFromSession(20, _session1);
                vergeRepo.AddOrUpdate(verge2);
                
                unitOfWork.Commit();
            }
        }

        [Test]
        public void ExpertRepository_GetExpertByNameAndSession()
        {
            using (UnitOfWorkFactory.Create()) {
                var expertRepository = new ExpertRepository(GetRepository<Expert>(), LinqProvider);

                var actualExpert = expertRepository.GetExpertByNameAndSession(
                    new GetExpertByNameAndSessionSpecification(_expert1.Name, _expert1.SessionOfExperts,
                        ExpertFetch.None));

                actualExpert.Should().BeEquivalentTo(_expert1, opt => opt.ExcludingNestedObjects());
            }
        }

        [Test]
        public void NodeRepository_GetSemanticNetworkBySession()
        {
            using (UnitOfWorkFactory.Create()) {
                var expertRepository = new NodeRepository(GetRepository<Node>(), LinqProvider);

                var actualSemanticNetwork = expertRepository.GetSemanticNetworkBySession(_session1);

                actualSemanticNetwork.Concepts.Should().BeEquivalentTo(new []
                {
                    new ConceptReadModel("notion1", "type",
                        new List<VergeReadModel>() {new VergeReadModel("notion2", "type", "notion1", "type", "type", 20)}, 
                        new List<VergeReadModel>() {new VergeReadModel("notion1", "type", "notion2", "type", "type", 20)}),

                    new ConceptReadModel("notion2", "type",
                        new List<VergeReadModel>() {new VergeReadModel("notion1", "type", "notion2", "type", "type", 20)},
                        new List<VergeReadModel>() {new VergeReadModel("notion2", "type", "notion1", "type", "type", 20)}),
                });
            }
        }
    }
}
