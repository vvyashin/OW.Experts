using System.Linq;
using Domain;
using FluentAssertions;
using NUnit.Framework;

namespace IntergrationTests.MappingTests
{
    [TestFixture]
    public class VergeTests : DropCreateOnSetupTestFixture
    {
        [Test]
        public void TestAddSession()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var notionType = new NotionType("type");
                GetRepository<NotionType>().AddOrUpdate(notionType);

                var node1 = new Node("notion1", notionType);
                GetRepository<Node>().AddOrUpdate(node1);

                var node2 = new Node("notion2", notionType);
                GetRepository<Node>().AddOrUpdate(node2);

                var relationType = new RelationType("type");
                GetRepository<RelationType>().AddOrUpdate(relationType);

                var verge = new Verge(node1, node2, relationType, 20);
                GetRepository<Verge>().AddOrUpdate(verge);

                unitOfWork.Commit();
            }

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var session = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(session);

                var verge = LinqProvider.Query<Verge>().Single();
                verge.UpdateWeightFromSession(20, session);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var verge = LinqProvider.Query<Verge>().Single();

                verge.SessionWeightSlices.Count.Should().Be(1);
            }
        }
    }
}
