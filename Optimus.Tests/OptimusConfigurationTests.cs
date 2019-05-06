using NUnit.Framework;
using Optimus.Exceptions;
using Optimus.Model;
using Shouldly;

namespace Optimus.Tests
{
    [TestFixture]
    public class OptimusConfigurationTests
    {
        [SetUp]
        public void BeforeEachTest() 
            => OptimusConfiguration.Delete(SuiteConfig.Repo);

        [Test]
        public void Json_will_be_created_if_it_doesnt_exist()
        {
            var (config, created) = OptimusConfiguration.GetOrCreate(SuiteConfig.Repo);
            
            created.ShouldBeTrue();
            config.ShouldNotBeNull();
            config.FileExtensions.ShouldNotBeEmpty();
            config.TinyPngApiKeys.ShouldBeEmpty();
        }

        [Test]
        public void Should_use_existing_config()
        {
            var (config, created) = OptimusConfiguration.GetOrCreate(SuiteConfig.Repo);
            created.ShouldBeTrue();

            var keys = new[] {"one", "two"};
            config.TinyPngApiKeys = keys;
            config.Save(SuiteConfig.Repo);
            
            (config, created) = OptimusConfiguration.GetOrCreate(SuiteConfig.Repo);
            created.ShouldBeFalse();
            config.TinyPngApiKeys.ShouldBe(keys);
        }

        [Test]
        public void Should_throw_if_file_extensions_missing()
        {
            var (config, created) = OptimusConfiguration.GetOrCreate(SuiteConfig.Repo);
            config.FileExtensions = null;
            config.Save(SuiteConfig.Repo);
            
            Should.Throw<OptimusConfigurationException>(
                () => OptimusConfiguration.GetOrCreate(SuiteConfig.Repo));
        }

        [Test]
        public void Should_throw_if_TinyPngApiKeys_missing()
        {
            OptimusConfiguration.GetOrCreate(SuiteConfig.Repo);

            Should.Throw<OptimusConfigurationException>(
                () => OptimusConfiguration.GetOrCreate(SuiteConfig.Repo));
        }
    }
}