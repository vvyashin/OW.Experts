using FluentAssertions;
using NUnit.Framework;

namespace OW.Experts.Test.Infrastructure.Tests
{
    [TestFixture]
    public class TestObjectExtensionTests
    {
        [Test]
        public void SetProperty_SetProtectedProperty()
        {
            var testObject = new ClassWithProtectedProperty()
                .SetProperty(nameof(ClassWithProtectedProperty.Name), "newName");

            testObject.Name.Should().Be("newName");
        }

        private class ClassWithProtectedProperty
        {
            public string Name { get; protected set; }
        }

        private class ClassWithPrivateField
        {
            public string Name { get; }
        }
    }
}