using System;
using FluentAssertions;
using NUnit.Framework;
using OW.Experts.Domain.Infrastructure;
using OW.Experts.Domain.Infrastructure.Extensions;
using OW.Experts.Test.Infrastructure;
using OW.Experts.Test.Infrastructure.FluentAssertions;

namespace OW.Experts.Domain.Tests
{
    [TestFixture]
    public class SessionOfExpertsTests
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\n")]
        [TestCase("\t  ")]
        public void Ctor_IfBaseNotionIsNullOrWhiteSpace_Throw(string notion)
        {
            var ex = Assert.Throws<ArgumentException>(() => new SessionOfExperts(notion));
            Assert.That(ex.Message.FirstLine(), Is.EqualTo("Notion should not be empty string"));
        }

        [TestCase(SessionPhase.MakingAssociations, SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes, SessionPhase.SelectingNodes)]
        [TestCase(SessionPhase.SelectingNodes, SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations, SessionPhase.Ended)]
        public void TestNextPhase(SessionPhase currentPhase, SessionPhase nextPhase)
        {
            var session = CreateSession()
                .SetProperty(nameof(SessionOfExperts.CurrentPhase), currentPhase);

            session.NextPhaseOrFinish();

            session.CurrentPhase.Should().Be(nextPhase);
        }

        [Test]
        public void Ctor_StartedIsNow_CurrenSessionIsMakingAssociations()
        {
            var session = new SessionOfExperts("baseNotion");

            session.CurrentPhase.Should().Be(SessionPhase.MakingAssociations);
            session.StartTime.Should().BeNow();
            session.BaseNotion.Should().Be("baseNotion");
        }

        [Test]
        public void Finish_SessionPhaseIsEnded()
        {
            var session = CreateSession();

            session.Finish();

            session.CurrentPhase.Should().Be(SessionPhase.Ended);
        }

        private SessionOfExperts CreateSession()
        {
            return new SessionOfExperts("baseNotion");
        }
    }
}