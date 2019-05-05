using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OW.Experts.Domain;

namespace OW.Experts.IntergrationTests.MappingTests
{
    [TestFixture]
    public class NodeTests : DropCreateOnSetupTestFixture
    {
        [Test]
        public void GetReadOnlyIngoingAndOutgoing()
        {
            Node node1, node2;
            Verge verge1, verge2;
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var notionType = new NotionType("type");
                GetRepository<NotionType>().AddOrUpdate(notionType);

                var nodeRepo = GetRepository<Node>();
                node1 = new Node("notion1", notionType);
                nodeRepo.AddOrUpdate(node1);

                node2 = new Node("notion2", notionType);
                nodeRepo.AddOrUpdate(node2);

                var relationType = new RelationType("type");
                GetRepository<RelationType>().AddOrUpdate(relationType);

                var vergeRepo = GetRepository<Verge>();
                verge1 = new Verge(node1, node2, relationType, 20);
                vergeRepo.AddOrUpdate(verge1);

                verge2 = new Verge(node2, node1, relationType, 20);
                vergeRepo.AddOrUpdate(verge2);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                node1 = GetRepository<Node>().GetById(node1.Id);
                node2 = GetRepository<Node>().GetById(node2.Id);

                node1.IngoingVerges.Select(x => x.Id).Should().BeEquivalentTo(new[] { verge2.Id });
                node1.OutgoingVerges.Select(x => x.Id).Should().BeEquivalentTo(new[] { verge1.Id });
                node2.IngoingVerges.Select(x => x.Id).Should().BeEquivalentTo(new[] { verge1.Id });
                node2.OutgoingVerges.Select(x => x.Id).Should().BeEquivalentTo(new[] { verge2.Id });
            }
        }

        [Test]
        public void TestAddSession()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var notionType = new NotionType("type");
                GetRepository<NotionType>().AddOrUpdate(notionType);

                var node = new Node("notion", notionType);
                GetRepository<Node>().AddOrUpdate(node);

                unitOfWork.Commit();
            }

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var session = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(session);

                var node = LinqProvider.Query<Node>().Single();
                node.AddSessionOfExperts(session);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var node = LinqProvider.Query<Node>().Single();

                node.SessionsOfExperts.Count.Should().Be(1);
            }
        }

        [Test]
        public void TestNewNode()
        {
            Node node;
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var notionType = new NotionType("type");
                GetRepository<NotionType>().AddOrUpdate(notionType);

                node = new Node("notion", notionType);
                GetRepository<Node>().AddOrUpdate(node);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var savedNode = GetRepository<Node>().GetById(node.Id);

                savedNode.Should().BeEquivalentTo(savedNode, opt => opt.ExcludingNestedObjects());
            }
        }
    }
}