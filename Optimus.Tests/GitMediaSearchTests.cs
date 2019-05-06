using System;
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
        private readonly string[] _imageExtensions = {".jpg", ".png"};

        [Test]
        [TestCase(".jpg", 4)]
        [TestCase(".png", 1)]
        [TestCase(".JPG", 4)]
        [TestCase(".PNG", 1)]
        public async Task Files_should_match_passed_extension(string extension, int count)
        {
            var extensions = new[] {extension};
            var search = (await new GitMediaSearch()
                .SearchMedia(SuiteConfig.RepoPath, extensions))
                .ToList();
            
            search.Count.ShouldBe(count);
            search.ShouldAllBe(file => Path.HasExtension(extension));
        }

        [Test]
        public async Task All_images_should_be_listed()
        {
            var search = await new GitMediaSearch()
                .SearchMedia(SuiteConfig.RepoPath, _imageExtensions);
            
            search.Count().ShouldBe(5);
        }

        [Test]
        public async Task Search_points_to_existing_files()
        {
            var search = (await new GitMediaSearch()
                .SearchMedia(SuiteConfig.RepoPath, _imageExtensions))
                .ToList();
            
            search.Count.ShouldBeGreaterThan(0);
            search.ShouldAllBe(path => File.Exists(path));
        }

        [Test]
        public async Task Search_in_directory_without_images_should_return_empty_enumerable()
        {
            var path = Path.Combine(SuiteConfig.RepoPath, "Dir4");
            var search = await new GitMediaSearch()
                    .SearchMedia(path, _imageExtensions);
            
            search.ShouldBeEmpty();
        }

        [Test]
        public void Search_should_throw_if_path_does_not_exist()
        {
            var path = Path.Combine(SuiteConfig.RepoPath, "IdontExist");
            Should.Throw<DirectoryNotFoundException>(
                () => new GitMediaSearch().SearchMedia(path, _imageExtensions));
        }

        [Test]
        public void Search_should_throw_if_no_extensions_passed()
        {
            var media = new GitMediaSearch();
            
            Should.Throw<ArgumentException>(
                () => media.SearchMedia(SuiteConfig.RepoPath, null));
            
            Should.Throw<ArgumentException>(
                () => media.SearchMedia(SuiteConfig.RepoPath, new string[]{}));
        }

        [Test]
        public void Extensions_should_start_with_dot()
        {
            var media = new GitMediaSearch();
            
            Should.Throw<ArgumentException>(
                () => media.SearchMedia(SuiteConfig.RepoPath, new []{"jpg"}));
        }
    }
}