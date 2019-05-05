using System;
using NUnit.Framework;

namespace OW.Experts.Domain.Tests
{
    [TestFixture]
    public class RelationTypeTests
    {
        [Test]
        public void Ctor()
        {
            var type = new RelationType("name");

            Assert.That(type.Name, Is.EqualTo("name"));
        }

        [Test]
        public void New_IfNameConsistsOfWhitespaces_Throw()
        {
            var ex = Assert.Throws<ArgumentException>(() => new RelationType(" \t\n"));
            Assert.That(ex.Message, Is.EqualTo("Name should not contains only whitespaces"));
        }

        [Test]
        public void New_IfNameIsNull_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new RelationType(null));
        }
    }
}