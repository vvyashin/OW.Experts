using System;
using Domain.Infrastructure.Extensions;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class AssociationTests
    {
        private Association CreateAssociation()
        {
            var expertFake = Substitute.For<Expert>();

            return new Association(expertFake, "notion");
        }

        [Test]
        public void Ctor_IfExpertIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new Association(null, "notion"));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("\n")]
        [TestCase("\t  ")]
        public void Ctor_IfNotionIsNullOrWhiteSpace_Throw(string notion)
        {
            var expertFake = Substitute.For<Expert>();

            var ex = Assert.Throws<ArgumentException>(() => new Association(expertFake, notion));
            Assert.That(ex.Message.FirstLine(), Is.EqualTo("Notion should not be empty string"));
        }

        [Test]
        public void UpdateTypes_IfTypeIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => CreateAssociation().UpdateTypes(null, "offer"));
        }

        [Test]
        public void UpdateTypes_UpdatedType()
        {
            var typeFake = Substitute.For<NotionType>();

            var association = CreateAssociation();
            association.UpdateTypes(typeFake, "offer");

            association.ShouldBeEquivalentTo(new {Type = typeFake, OfferType = "offer"},
                opt => opt.ExcludingMissingMembers());
        }
    }
}
