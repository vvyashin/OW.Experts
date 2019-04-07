using FluentAssertions;
using NUnit.Framework;
using OW.Experts.Domain.Infrastructure.Extensions;

namespace OW.Experts.Domain.Infrastructure.Tests
{
    [TestFixture]
    public class ObjectExtensionTests
    {
        [Test]
        public void TestEnumerate()
        {
            var obj = new object();

            obj.Enumerate().Should().ContainSingle(o => ReferenceEquals(o, obj));
        }
    }
}
