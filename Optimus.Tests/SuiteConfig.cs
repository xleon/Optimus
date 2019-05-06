using System.IO;
using System.IO.Compression;
using NUnit.Framework;

namespace Optimus.Tests
{
    [SetUpFixture]
    public class SuiteConfig
    {
        public const string RepoPath = "OptimusCompanionRepo";
        
        [OneTimeSetUp]
        public void BeforeTestSuite()
        {
            var repoZip = Path.Combine("TestHelpers", "OptimusCompanionRepo.zip");
            ZipFile.ExtractToDirectory(repoZip, ".");
        }

        [OneTimeTearDown]
        public void AfterTestSuite()
        {
            Directory.Delete(RepoPath, true);
        }
    }
}