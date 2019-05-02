using NUnit.Framework;
using Serilog;

namespace Optimus.Tests.TestHelpers
{
    public abstract class BaseTest
    {
        [SetUp]
        public virtual void BeforeEachTest()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug()
                .CreateLogger();
        }

        [TearDown]
        public virtual void AfterEachTest()
        {
            Log.CloseAndFlush();
        }
    }
}