using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class FileManagerTests
    {
        [Test]
        public async Task Find_images_should_make_recursive_search()
        {
            var includedDirectories = new[] {"Pockit.Mobile.Android", "Pockit.Mobile.iOS"};
            var files = await new GitMediaSearch().SearchImageFiles(@"C:\Users\xleon\Projects\Pockit", includedDirectories);

            var theFiles = files.ToList();
            
            foreach (var file in theFiles)
            {
                Debug.WriteLine(file);
            }
            
            theFiles.Count.ShouldBe(100);
        }

        [Test]
        public void WriteConfiguration()
        {
            var config = new OptimusConfiguration
            {
                FileExtensions = new[] {".jpg", ".jpeg", ".png"},
                TinyPngApiKeys = new[] {"123", "456"},
                IncludedDirectories = new[] {"Pockit.Mobile.Android", "Pockit.Mobile.iOS"}
            };
            
            config.SaveTo(@"C:\Users\xleon\Projects\Pockit");
        }

        [Test]
        public async Task Read()
        {
            const string path = @"C:\Users\xleon\Projects\Pockit";
            var config = OptimusConfiguration.GetOrCreate(path);
            var files = await new GitMediaSearch().SearchImageFiles(path, config.FileExtensions, config.IncludedDirectories);
            
            foreach (var file in files)
            {
                Debug.WriteLine(file);
            }
        }

        [Test]
        public async Task FileTracker()
        {
            var tracker = new OptimusFileTracker(@"C:\Users\xleon\Projects\Pockit", new GitMediaSearch());
            var (tracked, untracked) = await tracker.Track();

            foreach (var file in untracked)
            {
                Debug.WriteLine(file);
            }
        }

        [Test]
        public async Task Optimize()
        {
            var tracker = new OptimusFileTracker(@"C:\Users\xleon\Projects\Pockit", new GitMediaSearch());
            var (tracked, untracked) = await tracker.Track();
            var optimizer = new TestOptimizer();
            
            foreach (var file in untracked)
            {
                var temp = Path.Combine(Path.GetTempPath(), file);
                var optimized = await optimizer.Optimize(file, temp);
                Debug.WriteLine(optimized);
            }
        }
    }
}