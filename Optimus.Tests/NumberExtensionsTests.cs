using NUnit.Framework;
using Optimus.Helpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class NumberExtensionsTests
    {
        [Test]
        [TestCase(1024, "1Kb")]
        [TestCase(2048, "2Kb")]
        [TestCase(2100, "2Kb")]
        [TestCase(700, "700B")]
        [TestCase(1073741824, "1Gb")]
        public void ToFileLengthRepresentation(long amount, string expected)
        {
            amount.ToFileLengthRepresentation().ShouldBe(expected);
        }
    }
}