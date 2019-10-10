using HyperIoC.Tests.Support;
using Shouldly;
using Xunit;

namespace HyperIoC.Tests
{
    public class FactoryBuilderTests
    {
        [Fact]
        public void CreateBuildsFactoryForAnyConfig()
        {
            var factory = FactoryBuilder
                .Build()
                .WithProfile<AnyProfile>()
                .Create();

            var instance = factory.Get<ITestConfig>();

            instance.ShouldBeOfType<TestConfig>();
        }

        [Fact]
        public void CreateBuildsDebugFactory()
        {
            const string setting = "DEBUG";

            var factory = FactoryBuilder
                .Build()
                .WithProfile<DebugProfile>(() => setting == "DEBUG")
                .WithProfile<ReleaseProfile>(() => setting == "RELEASE")
                .Create();

            var instance = factory.Get<ITestClass>();
            
            instance.ShouldBeOfType<TestClass>();
        }

        [Fact]
        public void CreateBuildsReleaseFactory()
        {
            const string setting = "RELEASE";

            var factory = FactoryBuilder
                .Build()
                .WithProfile<DebugProfile>(() => setting == "DEBUG")
                .WithProfile<ReleaseProfile>(() => setting == "RELEASE")
                .Create();

            var instance = factory.Get<ITestClass>();

            instance.ShouldBeOfType<AnotherTestClass>();
        }

        [Fact]
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

            instance.ShouldBeOfType<AnotherTestClass>();
        }
    }
}
