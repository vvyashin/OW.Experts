using System;
using Domain.Infrastructure.Extensions;
using FluentAssertions;
using NUnit.Framework;

namespace Domain.Infrastructure.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void TestSubstring()
        {
            var returnSubstring = "returnSubstring";
            var tillValue = "till";

            var actual = (returnSubstring + tillValue).Substring(tillValue);

            actual.Should().Be(returnSubstring);
        }

        [Test]
        public void TestFirstLine()
        {
            var firstLine = "firstLine";
            var secondLine = "secondLine";

            var actual = (firstLine + Environment.NewLine + secondLine).FirstLine();

            actual.Should().Be(firstLine);
        }
    }
}
