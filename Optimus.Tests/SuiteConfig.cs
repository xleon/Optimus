using System.IO;
using System.IO.Compression;
using NUnit.Framework;

namespace Optimus.Tests
{
    [SetUpFixture]
    public class SuiteConfig
    {
        public const string Repo = "OptimusCompanionRepo";
        
        [OneTimeSetUp]
        public void BeforeTestSuite()
        {
            var repoZip = Path.Combine("TestHelpers", $"{Repo}.zip");
            ZipFile.ExtractToDirectory(repoZip, ".");
        }

        [OneTimeTearDown]
        public void AfterTestSuite()
        {
            Directory.Delete(Repo, true);
        }
    }
}