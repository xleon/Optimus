using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Optimus.Contracts;
using Optimus.Tests.TestHelpers;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class GitMediaSearchTests : BaseTest
    {
        private readonly string[] _imageExtensions = {".jpg", ".png"};
        private readonly IMediaSearch _mediaSearch = new GitMediaSearch();
        
        [Test]
        [TestCase(".jpg", 4)]
        [TestCase(".png", 1)]
        [TestCase(".JPG", 4)]
        [TestCase(".PNG", 1)]
        public async Task Result_should_match_passed_extension(string extension, int count)
        {
            var extensions = new[] {extension};
            var search = (await _mediaSearch
                .SearchMedia(SuiteConfig.Repo, extensions))
                .ToList();
            
            search.Count.ShouldBe(count);
            search.ShouldAllBe(file => Path.HasExtension(extension));
        }

        [Test]
        public async Task All_images_should_be_listed()
        {
            var search = await _mediaSearch.SearchMedia(SuiteConfig.Repo, _imageExtensions);
            search.Count().ShouldBe(5);
        }

        [Test]
        public async Task Results_point_to_existing_files()
        {
            var search = (await _mediaSearch
                .SearchMedia(SuiteConfig.Repo, _imageExtensions))
                .ToList();
            
            search.Count.ShouldBeGreaterThan(0);
            search.ShouldAllBe(path => 
                File.Exists(Path.Combine(SuiteConfig.Repo, path)));
        }

        [Test]
        public async Task Search_in_directory_without_images_should_return_empty_enumerable()
        {
            var path = Path.Combine(SuiteConfig.Repo, "Dir4");
            var search = await _mediaSearch.SearchMedia(path, _imageExtensions);
            
            search.ShouldBeEmpty();
        }

        [Test]
        public void Search_should_throw_if_path_does_not_exist()
        {
            var path = Path.Combine(SuiteConfig.Repo, "IdontExist");
            Should.Throw<DirectoryNotFoundException>(
                () => new GitMediaSearch().SearchMedia(path, _imageExtensions));
        }

        [Test]
        public void Search_should_throw_if_no_extensions_passed()
        {
            Should.Throw<ArgumentException>(
                () => _mediaSearch.SearchMedia(SuiteConfig.Repo, null));
            
            Should.Throw<ArgumentException>(
                () => _mediaSearch.SearchMedia(SuiteConfig.Repo, new string[]{}));
        }

        [Test]
        public void Extensions_should_start_with_dot()
        {
            Should.Throw<ArgumentException>(
                () => _mediaSearch.SearchMedia(SuiteConfig.Repo, new []{"jpg"}));
        }
    }
}