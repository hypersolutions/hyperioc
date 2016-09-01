using HyperIoC;
#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Tests.HyperIoC.Support;

namespace Tests.HyperIoC
{
    [TestClass]
    public class FactoryBuilderTests
    {
        [TestMethod]
        public void CreateBuildsFactoryForAnyConfig()
        {
            var factory = FactoryBuilder
                .Build()
                .WithProfile<AnyProfile>()
                .Create();

            var instance = factory.Get<ITestConfig>();

            Assert.IsInstanceOfType(instance, typeof(TestConfig));
        }

        [TestMethod]
        public void CreateBuildsDebugFactory()
        {
            const string setting = "DEBUG";

            var factory = FactoryBuilder
                .Build()
                .WithProfile<DebugProfile>(() => setting == "DEBUG")
                .WithProfile<ReleaseProfile>(() => setting == "RELEASE")
                .Create();

            var instance = factory.Get<ITestClass>();

            Assert.IsInstanceOfType(instance, typeof(TestClass));
        }

        [TestMethod]
        public void CreateBuildsReleaseFactory()
        {
            const string setting = "RELEASE";

            var factory = FactoryBuilder
                .Build()
                .WithProfile<DebugProfile>(() => setting == "DEBUG")
                .WithProfile<ReleaseProfile>(() => setting == "RELEASE")
                .Create();

            var instance = factory.Get<ITestClass>();

            Assert.IsInstanceOfType(instance, typeof(AnotherTestClass));
        }

        [TestMethod]
        public void CreateBuildsFactoryFromAnother()
        {
            var originalFactory = FactoryBuilder
                .Build()
                .WithProfile<AnyProfile>()
                .Create();
            var factory = FactoryBuilder
                .Build(originalFactory)
                .WithProfile<DebugProfile>(() => originalFactory.Get<ITestConfig>().Config == "DEBUG")
                .WithProfile<ReleaseProfile>(() => originalFactory.Get<ITestConfig>().Config == "RELEASE")
                .Create();

            var instance = factory.Get<ITestClass>();

            Assert.IsInstanceOfType(instance, typeof(AnotherTestClass));
        }
    }
}
