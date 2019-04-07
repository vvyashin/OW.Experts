using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class VergeTests
    {
        [TestCase(40, 50, 45)]
        [TestCase(32, 35, 33)]
        public void TestUpdateWeight_SaveAverageWeight(int oldWeight, int addedWeight, int resultWeight)
        {
            var verge = new Verge(Substitute.For<Node>(), Substitute.For<Node>(), 
                Substitute.For<RelationType>(), oldWeight);

            var newVerge = verge.UpdateWeightFromSession(addedWeight, Substitute.For<SessionOfExperts>());

            newVerge.Weight.Should().Be(resultWeight);
        }

        [Test]
        public void TestUpdateWeigth_AddSession()
        {
            var verge = new Verge(Substitute.For<Node>(), Substitute.For<Node>(), 
                Substitute.For<RelationType>(), 20);
            var session = Substitute.For<SessionOfExperts>();

            verge.UpdateWeightFromSession(20, session);

            verge.SessionWeightSlices.Should().BeEquivalentTo(new[] { new { Session = session, Verge = verge, Weight = 20 }},
                opt => opt.ExcludingMissingMembers());
        }
    }
}
