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
    public class TinyPngOptimizerTests : BaseTest
    {
        private const string Key1 = "EqMJnscp5i6PXT2bLuV36ktbvCD11lCi";
        
        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            Tinify.Key = null;

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
        public void ApiKey_should_be_provided_in_constructor()
        {
            Should.Throw<ArgumentException>(() => new TinyPngOptimizer(null));
            Should.Throw<ArgumentException>(() => new TinyPngOptimizer(new string[]{}));
            Should.Throw<ArgumentException>(() => new TinyPngOptimizer(new []{""}));
            Should.Throw<ArgumentException>(() => new TinyPngOptimizer(new []{" "}));
            Should.Throw<ArgumentException>(() => new TinyPngOptimizer(new []{"asd", ""}));
            Should.Throw<ArgumentException>(() => new TinyPngOptimizer(new []{"asd", "  "}));
        }

        [Test]
        [Category("Integration")]
        public async Task Should_optimize_if_any_key_is_valid()
        {
            var request = GetRequest("arandano.jpg");
            var optimizer = new TinyPngOptimizer(new []{"bad1", "bad2", Key1});
            var result = await optimizer.Optimize(request);
            
            result.Success.ShouldBeTrue();
            Tinify.Key.ShouldBe(Key1);
            
            optimizer = new TinyPngOptimizer(new []{"bad1", Key1, "bad2"});
            result = await optimizer.Optimize(request);
            
            result.Success.ShouldBeTrue();
            Tinify.Key.ShouldBe(Key1);
        }

        [Test]
        [Category("Integration")]
        public void Optimize_should_throw_when_all_keys_invalid()
        {
            var request = GetRequest("arandano.jpg");
            var optimizer = new TinyPngOptimizer(new []{"bad1", "bad2", "bad3"});

            Should.Throw<ApiAccessException>(() => optimizer.Optimize(request));
            Tinify.Key.ShouldBe("bad3");
        }

        [Test]
        [Category("Integration")]
        public async Task Optimize_non_image_file_should_fail()
        {
            var result = await new TinyPngOptimizer(new []{Key1})
                .Optimize(GetRequest("file.txt"));
            
            result.Success.ShouldBeFalse();
            result.ErrorMessage.ShouldContain("HTTP 415/Unsupported media type");
        }

        [Test]
        [Category("Integration")]
        public async Task Image_file_should_be_optimized()
        {
            var request = GetRequest("arandano.jpg");
            var result = await new TinyPngOptimizer(new []{Key1})
                .Optimize(request);

            result.Success.ShouldBeTrue();
            result.OriginalLength.ShouldBe(request.Length);
            result.FilePath.ShouldBe(request.FilePath);
            result.Length.ShouldBeLessThan(request.Length);
            result.Length.ShouldNotBe(0);
            result.ErrorMessage.ShouldBeNull();
        }

        private static OptimizeRequest GetRequest(string file)
            => new OptimizeRequest(Path.Combine("TestImages", file));
    }
}