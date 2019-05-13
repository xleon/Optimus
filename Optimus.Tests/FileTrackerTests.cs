using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Optimus.Contracts;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class FileTrackerTests
    {
        private readonly IFileTracker _tracker;
        private readonly Mock<IMediaSearch> _mediaMock;
        private readonly string[] _paths;
        
        public FileTrackerTests()
        {
            _paths = new[]
            {
                "Dir1/inner/azúcar.jpg",
                "Dir1/garbanzos.png",
                "Dir2/judias.jpg",
                "Dir2/leche.jpg",
                "Dir3/centeno.jpg"
            };
            
            _mediaMock = new Mock<IMediaSearch>();
            
            _mediaMock
                .Setup(x => x.SearchMedia(
                    It.Is<string>(s => s == SuiteConfig.Repo), 
                    It.IsAny<string[]>()))
                .ReturnsAsync(() => _paths);
            
            _tracker = new FileTracker(SuiteConfig.Repo, _mediaMock.Object);
        }
        
        [SetUp]
        public void BeforeEachTest() 
            => _tracker.Untrack();

        [Test]
        public void Should_throw_if_directory_path_missing()
        {
            Should.Throw<ArgumentException>(
                () => new FileTracker(null, _mediaMock.Object));
            
            Should.Throw<ArgumentException>(
                () => new FileTracker("", _mediaMock.Object));
            
            Should.Throw<ArgumentException>(
                () => new FileTracker("IdontExist", _mediaMock.Object));
        }

        [Test]
        public void Should_throw_if_media_search_missing() 
            => Should.Throw<ArgumentException>(
                () => new FileTracker(SuiteConfig.Repo, null));

        [Test]
        public async Task Should_report_untracked_files()
        {
            var report = (await _tracker.Report()).ToList();
            
            report.Count.ShouldBe(5);
            report.ShouldAllBe(x => !x.Tracked);
        }

        [Test]
        public async Task Files_can_be_tracked()
        {
            await _tracker.Report();
            _paths.ToList().ForEach(x => _tracker.Track(x));

            var report = (await _tracker.Report()).ToList();
            
            report.ShouldAllBe(x => x.Tracked);
            report.Count.ShouldBe(_paths.Length);
        }

        [Test]
        public void Report_should_be_generated_before_tracking_files()
        {
            Should.Throw<InvalidOperationException>(() =>
                _tracker.Track(Path.Combine("Dir3", "centeno.jpg")));
        }
        
        [Test]
        public async Task Should_not_track_files_out_of_report()
        {
            await _tracker.Report();
            Should.Throw<ArgumentException>(() =>
                _tracker.Track(Path.Combine("bin", "notInMediaSearch.jpg")));
        }

        [Test]
        public async Task File_track_should_not_be_duplicated()
        {
            await _tracker.Report();
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

        [Test]
        public async Task Can_set_all_files_as_tracked()
        {
            var fileInfos = (await _tracker.SetAllFilesAsTracked()).ToList();

            fileInfos.Count.ShouldBe(_paths.Length);
            fileInfos.ShouldAllBe(x => x.Tracked);
        }
    }
}