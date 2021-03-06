using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class OptimusFileTrackerTests
    {
        private OptimusFileTracker _tracker;
        
        [SetUp]
        public void BeforeEachTest()
        {
            _tracker = new OptimusFileTracker(SuiteConfig.Repo);

            var fileInfo = _tracker.GetTrackerFileInfo();
            
            if(fileInfo.Exists)
                fileInfo.Delete();
        }

        [Test]
        public async Task Track_should_throw_if_files_does_not_exists()
        {
            await Should.ThrowAsync<FileNotFoundException>(() => _tracker.Track("fake.jpg"));
        }
        
        [Test]
        public async Task Track_should_add_line_to_file_tracker_and_order_lines_by_path()
        {
            await _tracker.Track("Dir3/centeno.jpg");
            await _tracker.Track("Dir2/azafran.jpg");
            await _tracker.Track("Dir2/Subdir/cabrales.jpg");
            await _tracker.Track("Dir1/icon.png");
            await _tracker.Track("Dir1/avellana.jpg");

            var tracks = (await _tracker.GetTrackInfos()).ToList();
            
            tracks.ShouldAllBe(x => !x.Updated);
            tracks.ShouldAllBe(x => x.FileHash != null);
            
            tracks[0].RelativePath.ShouldBe("Dir1/avellana.jpg");
            tracks[1].RelativePath.ShouldBe("Dir1/icon.png");
            tracks[2].RelativePath.ShouldBe("Dir2/azafran.jpg");
            tracks[3].RelativePath.ShouldBe("Dir2/Subdir/cabrales.jpg");
            tracks[4].RelativePath.ShouldBe("Dir3/centeno.jpg");
        }

        [Test]
        public async Task Track_should_update_hash_when_file_already_tracked()
        {
            File.Copy(
                Path.Combine(SuiteConfig.Repo, "Dir1/avellana.jpg"), 
                Path.Combine(SuiteConfig.Repo, "copy.jpg"));

            var info = await _tracker.Track("copy.jpg");
            var hash = info.FileHash;
            
            File.Copy(
                Path.Combine(SuiteConfig.Repo, "Dir3/centeno.jpg"), 
                Path.Combine(SuiteConfig.Repo, "copy.jpg"), 
                true);

            var result = await _tracker.Track("copy.jpg");
            result.Updated.ShouldBeTrue();
            result.FileHash.ShouldNotBeNull();
            result.FileHash.ShouldNotBe(hash);
        }

        [Test]
        public async Task Track_should_not_update_file_with_same_hash()
        {
            var trackInfo = await _tracker.Track("Dir1/avellana.jpg");
            
            trackInfo.Updated.ShouldBeTrue();
            
            trackInfo = await _tracker.Track("Dir1/avellana.jpg");
            
            trackInfo.Updated.ShouldBeFalse();
        }

        [Test]
        public async Task UntrackRemovedAndChangedFiles_should_update_tracker_file_by_removing_unexisting_files()
        {
            var trackerFile = Path.Combine(SuiteConfig.Repo, OptimusFileTracker.FileName);
            
            File.WriteAllLines(trackerFile, new []
            {
                "[123456789] Dir1/fake.png"
            });

            await _tracker.Track("Dir1/avellana.jpg");

            await _tracker.UntrackRemovedAndChangedFiles();
            
            var tracks = (await _tracker.GetTrackInfos()).ToList();
            
            tracks.Count.ShouldBe(1);
            tracks.First().RelativePath.ShouldBe("Dir1/avellana.jpg");
        }

        [Test]
        public async Task GetUntrackedPaths_should_return_paths_not_tracked()
        {
            await _tracker.Track("Dir1/avellana.jpg");

            var untracked = (await _tracker.GetUntrackedPaths(new[] {"/Dir1/untracked.png"})).ToList()
                .ToList();
            
            untracked.Count.ShouldBe(1);
            untracked.First().ShouldBe("/Dir1/untracked.png");
        }
        
        [Test]
        public void UntrackAll_should_delete_tracker_file()
        {
            var trackerFile = Path.Combine(SuiteConfig.Repo, OptimusFileTracker.FileName);
            File.WriteAllText(trackerFile, "[2020-01-01] Dir1/avellana.jpg");
            File.Exists(trackerFile).ShouldBeTrue();

            _tracker.UntrackAll();
            
            File.Exists(trackerFile).ShouldBeFalse();
        }
    }
}