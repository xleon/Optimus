using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class FileTrackerTests
    {
        [SetUp]
        public void BeforeEachTest()
        {
            new FileTracker(SuiteConfig.Repo, new GitMediaSearch())
                .Delete();
        }
        
        [Test]
        public void Should_throw_if_directory_path_missing()
        {
            var mediaSearch = new GitMediaSearch();
            
            Should.Throw<ArgumentException>(
                () => new FileTracker(null, mediaSearch));
            
            Should.Throw<ArgumentException>(
                () => new FileTracker("", mediaSearch));
            
            Should.Throw<ArgumentException>(
                () => new FileTracker("IdontExist", mediaSearch));
        }

        [Test]
        public void Should_throw_if_media_search_missing() 
            => Should.Throw<ArgumentException>(
                () => new FileTracker(SuiteConfig.Repo, null));

        [Test]
        public async Task Should_track_all_untracked_images()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        public async Task Should_report_outdated_files()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task Should_untrack_removed_files()
        {
            throw new NotImplementedException();
        }
    }
}