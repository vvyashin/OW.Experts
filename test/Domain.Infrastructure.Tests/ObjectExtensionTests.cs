using Domain.Infrastructure.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.Infrastructure.Tests
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
