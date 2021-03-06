﻿using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain.Tests
{
    [TestFixture]
    public class AssociationTests
    {
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
        public void Ctor_IfExpertIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new Association(null, "notion"));
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

            association.Should().BeEquivalentTo(
                new { Type = typeFake, OfferType = "offer" },
                opt => opt.ExcludingMissingMembers());
        }

        private Association CreateAssociation()
        {
            var expertFake = Substitute.For<Expert>();

            return new Association(expertFake, "notion");
        }
    }
}