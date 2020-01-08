using System;
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
        public void Track_should_throw_if_files_does_not_exists()
        {
            Should.ThrowAsync<ArgumentException>(() => _tracker.Track("fake.jpg"));
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
            tracks.ShouldAllBe(x => x.LastWriteTimeUtc < DateTime.UtcNow);
            
            tracks[0].RelativePath.ShouldBe("Dir1/avellana.jpg");
            tracks[1].RelativePath.ShouldBe("Dir1/icon.png");
            tracks[2].RelativePath.ShouldBe("Dir2/azafran.jpg");
            tracks[3].RelativePath.ShouldBe("Dir2/Subdir/cabrales.jpg");
            tracks[4].RelativePath.ShouldBe("Dir3/centeno.jpg");
        }

        [Test]
        public async Task Track_should_update_lastWriteTimeUtc_when_file_already_tracked()
        {
            var file = Path.Combine(SuiteConfig.Repo, "Dir1/avellana.jpg");
            var newFile = Path.Combine(SuiteConfig.Repo, "copy.jpg");
            File.Copy(file, newFile);

            var info = await _tracker.Track("copy.jpg");

            File.SetLastWriteTimeUtc(newFile, DateTime.UtcNow);

            var result = await _tracker.Track("copy.jpg");
            
            result.Updated.ShouldBeTrue();
            result.LastWriteTimeUtc.ShouldNotBe(info.LastWriteTimeUtc);
        }

        [Test]
        public async Task Track_should_not_update_file_with_same_lastWriteTimeUtc()
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
                "[2020-01-01] Dir1/fake.png"
            });

            await _tracker.Track("Dir1/avellana.jpg");

            await _tracker.UntrackRemovedAndChangedFiles();
            
            var tracks = (await _tracker.GetTrackInfos()).ToList();
            
            tracks.Count.ShouldBe(1);
            tracks.First().RelativePath.ShouldBe("Dir1/avellana.jpg");
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