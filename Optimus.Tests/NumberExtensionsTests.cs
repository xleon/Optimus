using NUnit.Framework;
using Optimus.Helpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class NumberExtensionsTests
    {
        [Test]
        [TestCase(1024, "1 KB")]
        [TestCase(2048, "2 KB")]
        [TestCase(2100, "2 KB")]
        [TestCase(700, "700 B")]
        [TestCase(1073741824, "1 GB")]
        public void ToFileLengthRepresentation(long amount, string expected)
        {
            amount.ToFileLengthRepresentation().ShouldBe(expected);
        }
    }
}