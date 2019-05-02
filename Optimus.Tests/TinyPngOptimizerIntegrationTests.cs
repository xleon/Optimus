using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Optimus.Exceptions;
using Optimus.Model;
using Optimus.Optimizers;
using Optimus.Tests.TestHelpers;
using Shouldly;
using TinifyAPI;

namespace Optimus.Tests
{
    [TestFixture]
    public class TinyPngOptimizerIntegrationTests : BaseTest
    {
        private const string Key1 = "EqMJnscp5i6PXT2bLuV36ktbvCD11lCi";
        
        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            var sourcePath = Path.Combine(Environment.CurrentDirectory, "Images");
            var targetPath = Path.Combine(Environment.CurrentDirectory, "TestImages");
            
            Directory.CreateDirectory(targetPath);

            foreach (var path in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var newPath = path.Replace(sourcePath, targetPath);
                File.Copy(path, newPath, true);
            }
        }

        [TearDown]
        public override void AfterEachTest()
        {
            base.AfterEachTest();
            
            var targetPath = Path.Combine(Environment.CurrentDirectory, "TestImages");
            Directory.Delete(targetPath, true);
        }
        
        [Test]
        public void Invalid_api_key_should_throw()
        {
            var optimizer = new TinyPngOptimizer(new []{"wrongKey"});
            Should.Throw<ApiAccessException>(() => optimizer.Initialize());
        }

        [Test]
        public async Task Initialize_should_take_first_valid_apikey()
        {
            var optimizer = new TinyPngOptimizer(new []{"wrongKey", "wrongKey2", Key1});
            await optimizer.Initialize();
            
            Tinify.Key.ShouldBe(Key1);
        }

        [Test]
        public async Task Next_apikey_should_be_taken_on_access_exception()
        {
            var optimizer = new TinyPngOptimizer(new []{"wrongKey", "wrongKey2", "wrongKey3"});
            var request = new OptimizeRequest(Path.Combine("TestImages", "arandano.jpg"));

            Should.Throw<ApiAccessException>(() => optimizer.Optimize(request));
            Tinify.Key.ShouldBe("wrongKey3");
            
            optimizer = new TinyPngOptimizer(new []{"wrongKey"});
            Should.Throw<ApiAccessException>(() => optimizer.Optimize(request));
            Tinify.Key.ShouldBe("wrongKey");
            
            optimizer = new TinyPngOptimizer(new []{"wrongKey", "wrongKey2", Key1});
            var result = await optimizer.Optimize(request);
            result.Success.ShouldBeTrue();
        }
        
        [Test]
        public async Task Non_image_should_fail()
        {
            var optimizer = new TinyPngOptimizer(new []{Key1});
            var request = new OptimizeRequest(Path.Combine("TestImages", "file.txt"));
            var result = await optimizer.Optimize(request);
            
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldNotBeEmpty();
        }

        [Test]
        public async Task Image_should_be_optimized()
        {
            var optimizer = new TinyPngOptimizer(new []{Key1});
            var request = new OptimizeRequest(Path.Combine("TestImages", "arandano.jpg"));
            var result = await optimizer.Optimize(request);

            result.Success.ShouldBeTrue();
            result.FilePath.ShouldBe(request.FilePath);
            result.Length.ShouldBeLessThan(request.Length);
            result.Length.ShouldNotBe(0);
            result.ErrorMessage.ShouldBeNull();
        }
    }
}