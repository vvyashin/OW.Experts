using System.Linq;
using Domain;
using Domain.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using Test.Infrastructure;

namespace IntergrationTests.MappingTests
{
    [TestFixture]
    public class SessionOfExpertsTests : DropCreateOnSetupTestFixture
    {
        [SetUp]
        public void TimeContextSetup()
        {
            TimeContext.Current = new FakeTimeContext();
        }

        [OneTimeTearDown]
        public void TimeContextTearDown()
        {
            TimeContext.Reset();
        }
        
        [Test]
        public void NewSessionAndUpdatePhase()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("BaseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                unitOfWork.Commit();
            }

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var session = LinqProvider.Query<SessionOfExperts>().Single();
                session.NextPhaseOrFinish();
                GetRepository<SessionOfExperts>().AddOrUpdate(session);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var session = LinqProvider.Query<SessionOfExperts>().Single();

                session.CurrentPhase.Should().Be(SessionPhase.SpecifyingAssociationsTypes);
                session.StartTime.Should().BeNow();
                session.BaseNotion.Should().Be("BaseNotion");
            }
        }
    }
}
