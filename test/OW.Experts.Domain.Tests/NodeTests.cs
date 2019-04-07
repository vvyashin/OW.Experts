using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace OW.Experts.Domain.Tests
{
    [TestFixture]
    public class NodeTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\t\n")]
        public void Ctor_IfNotionIsEmpty_Throw(string notion)
        {
            Assert.Throws<ArgumentException>(() => new Node(notion, Substitute.For<NotionType>()));
        }

        [Test]
        public void Equals_DifferentTypes_ReturnFalsse()
        {
            var node = new Node("notion", Substitute.For<NotionType>());
            var sessionOfExpert = new SessionOfExperts("notion");

            var result = node.Equals(sessionOfExpert);

            result.Should().BeFalse();
        }

        [Test]
        public void Equals_TypesDoesNotEquals_False()
        {
            var node = new Node("notion", Substitute.For<NotionType>());
            var node2 = new Node("notion", Substitute.For<NotionType>());

            var result = node.Equals(node2);

            result.Should().BeFalse();
        }

        [Test]
        public void Equals_NotionsDoesNotEquals_False()
        {
            var notionType = Substitute.For<NotionType>();
            var node = new Node("notion", notionType);
            var node2 = new Node("notion2", notionType);

            var result = node.Equals(node2);

            result.Should().BeFalse();
        }

        [TestCase("notion", "NotIOn")]
        [TestCase("notion", "notion")]
        public void Equals_NotionsAreEqualsIgnoreCase_False(string node1Notion, string node2Notion)
        {
            var notionType = Substitute.For<NotionType>();
            var node = new Node(node1Notion, notionType);
            var node2 = new Node(node2Notion, notionType);

            var result = node.Equals(node2);

            result.Should().BeTrue();
        }

        [Test]
        public void TestAddSessionOfExperts()
        {
            var node = new Node("notion", Substitute.For<NotionType>());
            var session = Substitute.For<SessionOfExperts>();

            node.AddSessionOfExperts(session);

            node.SessionsOfExperts.Should().BeEquivalentTo(new[] {session});
        }
    }
}
