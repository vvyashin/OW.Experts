using System;
using NUnit.Framework;

namespace OW.Experts.Domain.Tests
{
    [TestFixture]
    public class NotionTypeTests
    {
        [Test]
        public void Ctor()
        {
            var type = new NotionType("name");

            Assert.That(type.Name, Is.EqualTo("name"));
        }

        [Test]
        public void New_IfNameConsistsOfWhitespaces_Throw()
        {
            var ex = Assert.Throws<ArgumentException>(() => new NotionType(" \t\n"));
            Assert.That(ex.Message, Is.EqualTo("Name should not contains only whitespaces"));
        }

        [Test]
        public void New_IfNameIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new NotionType(null));
        }
    }
}