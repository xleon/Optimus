using System.IO;
using NUnit.Framework;
using Optimus.Model;
using Optimus.Tests.TestHelpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class ModelTests : BaseTest
    {
        [Test]
        public void Request_should_reference_existing_file()
        {
            Should.Throw<FileNotFoundException>(() => new OptimizeRequest("fake"));
            Should.NotThrow(() => new OptimizeResult(true, Path.Combine("Images", "arandano.jpg")));
        }

        [Test]
        public void Request_should_read_file_length()
        {
            var path = Path.Combine("Images", "arandano.jpg");
            var request = new OptimizeRequest(path);
            
            request.Length.ShouldBe(new FileInfo(path).Length);
        }

        [Test]
        public void Result_should_reference_existing_file()
        {
            Should.Throw<FileNotFoundException>(() => new OptimizeResult(true, "fake"));
            Should.NotThrow(() => new OptimizeResult(true, Path.Combine("Images", "arandano.jpg")));
        }
        
        [Test]
        public void Result_should_read_file_length()
        {
            var path = Path.Combine("Images", "arandano.jpg");
            var result = new OptimizeResult(true, path);
            
            result.Length.ShouldBe(new FileInfo(path).Length);
        }
    }
}