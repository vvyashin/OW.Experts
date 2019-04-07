using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace OW.Experts.Domain.Infrastructure.Tests
{
    [TestFixture]
    public class DomainObjectTests
    {
        [Test]
        public void Equals_IfIdAreEqual_True()
        {
            var domainObject = Substitute.ForPartsOf<DomainObject>();
            domainObject.Id.Returns(Guid.Parse("48ab0c2c-5bbd-47aa-bc60-aa4e16f3e86c"));

            var domainObjectWithEqualId = Substitute.ForPartsOf<DomainObject>();
            domainObjectWithEqualId.Id.Returns(Guid.Parse("48ab0c2c-5bbd-47aa-bc60-aa4e16f3e86c"));

            domainObject.Equals(domainObjectWithEqualId)
                .Should()
                .Be(true, because: "objects are equal");
        }
    }
}
