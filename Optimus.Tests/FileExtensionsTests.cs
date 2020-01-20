using System.IO;
using NUnit.Framework;
using Optimus.Helpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class FileExtensionsTests
    {
        [Test]
        public void CalculateMd5()
        {
            Path
                .Combine("Images", "arandano.jpg")
                .CalculateMd5()
                .ShouldBe("d6162ae75955523477c900ff8f31da68");
        }
    }
}