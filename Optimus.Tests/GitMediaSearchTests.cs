using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Optimus.Tests.TestHelpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class GitMediaSearchTests : BaseTest
    {
        private string _path;
        private string[] _imageExtensions;

        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            _path = "";
            _imageExtensions = new[] {".jpg", ".png"};
        }

        [TearDown]
        public override void AfterEachTest()
        {
            base.AfterEachTest();
            
//            var targetPath = Path.Combine(Environment.CurrentDirectory, "TestImages");
//            Directory.Delete(targetPath, true);
        }
        
        [Test]
        [TestCase(".jpg")]
        [TestCase(".png")]
        [TestCase(".JPG")]
        [TestCase(".PNG")]
        public async Task Files_should_match_passed_extension(string extension)
        {
            var extensions = new[] {extension};
            var search = (await new GitMediaSearch()
                .SearchImageFiles(_path, extensions))
                .ToList();
            
            search.Count.ShouldBeGreaterThan(0);
            search.ShouldAllBe(file => Path.HasExtension(extension));
        }

        [Test]
        public async Task All_images_should_be_listed()
        {
            var search = await new GitMediaSearch()
                .SearchImageFiles(_path, _imageExtensions);
            
            search.Count().ShouldBe(5);
        }

        [Test]
        public async Task Search_points_to_existing_files()
        {
            var search = (await new GitMediaSearch()
                .SearchImageFiles(_path, _imageExtensions))
                .ToList();
            
            search.Count.ShouldBeGreaterThan(0);
            search.ShouldAllBe(path => File.Exists(path));
        }

        [Test]
        public async Task Search_can_be_filtered_by_includeDirectories()
        {
            var includeDirectories = new[] {"Dir1", "Dir2"};
            var search = await new GitMediaSearch()
                .SearchImageFiles(_path, _imageExtensions, includeDirectories);
            
            search.Count().ShouldBe(4);
        }
    }
}