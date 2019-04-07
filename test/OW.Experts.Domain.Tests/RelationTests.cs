using System;
using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain.Tests
{
    [TestFixture]
    public class RelationTests
    {
        private Relation CreateRelation()
        {
            var expertFake = Substitute.For<Expert>();
            var sourceNodeFake = Substitute.For<Node>();
            var destintationNodeFake = Substitute.For<Node>();

            return new Relation(expertFake, sourceNodeFake, destintationNodeFake);
        }

        private class RelationWithTypes : Relation
        {
            public RelationWithTypes(Expert expert, Node source, Node destination, 
                IEnumerable<RelationType> types) : base(expert, source, destination)
            {
                _types.AddRange(types);
            }
        }

        private Relation CreateRelation(IEnumerable<RelationType> types)
        {
            var expertFake = Substitute.For<Expert>();
            var sourceNodeFake = Substitute.For<Node>();
            var destintationNodeFake = Substitute.For<Node>();

            return new RelationWithTypes(expertFake, sourceNodeFake, destintationNodeFake, types);
        }

        [Test]
        public void UpdateTypes_IfTypeIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => CreateRelation().UpdateTypes(null, ""));
        }

        [Test]
        public void TestUpdateTypes()
        {
            var oldTypeFake = Substitute.For<RelationType>();
            var relation = CreateRelation(oldTypeFake.Enumerate());
            var newTypeFake = Substitute.For<RelationType>();

            relation.UpdateTypes(newTypeFake.Enumerate(), "offer");

            relation.Should().BeEquivalentTo(new { IsChosen = ChosenState.HadChosen, Types = newTypeFake.Enumerate(), OfferType = "offer" },
                opt => opt.ExcludingMissingMembers());
        }

        [Test]
        public void Ctor_RelationHadNotChosendAndHastEmptyTypeCollection()
        {
            var relation = new Relation(Substitute.For<Expert>(), Substitute.For<Node>(), Substitute.For<Node>());

            relation.Should().BeEquivalentTo(new {IsChosen = ChosenState.HadNotChosen, Types = new List<RelationType>()},
                opt => opt.ExcludingMissingMembers());
        }
    }
}
