using FluentAssertions;
using NUnit.Framework;

namespace Test.Infrastructure.Tests
{
    [TestFixture]
    public class TestObjectExtensionTests
    {
        private class ClassWithProtectedProperty
        {
            public string Name { get; protected set; }
        }

        private class ClassWithPrivateField
        {
            #pragma warning disable 649
            private string _name;
            #pragma warning restore 649

            public string Name => _name;
        }

        [Test]
        public void SetProperty_SetProtectedProperty()
        {
            var testObject = new ClassWithProtectedProperty()
                .SetProperty(nameof(ClassWithProtectedProperty.Name), "newName");

            testObject.Name.Should().Be("newName");
        }
    }
}
