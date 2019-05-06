using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class FileTrackerTests
    {
        [Test]
        public void Should_throw_if_directory_path_missing()
        {
            var mediaSearch = new GitMediaSearch();
            
            Should.Throw<ArgumentException>(() => new FileTracker(null, mediaSearch));
            Should.Throw<ArgumentException>(() => new FileTracker("", mediaSearch));
            Should.Throw<ArgumentException>(() => new FileTracker("IdontExist", mediaSearch));
        }

        [Test]
        public void Should_throw_if_media_search_missing() 
            => Should.Throw<ArgumentException>(() => new FileTracker(SuiteConfig.Repo, null));

        [Test]
        public async Task Should_track_all_untracked_images()
        {
            throw new NotImplementedException();
        }
    }
}