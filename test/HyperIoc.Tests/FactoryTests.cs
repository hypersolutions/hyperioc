using System;
using System.Linq;
using HyperIoC.Lifetime;
using HyperIoC.Tests.Support;
using Shouldly;
using Xunit;
// ReSharper disable AssignNullToNotNullAttribute

namespace HyperIoC.Tests
{
    public class FactoryTests
    {
        private Factory _factory;

        public FactoryTests()
        {
            _factory = new Factory();
        }

        [Fact]
        public void AddThrowsExceptionNullInterfaceType()
        {
            var exception = Should.Throw<ArgumentNullException>(() => _factory.Add(null, typeof(TestClass)));
            
            exception.Message.ShouldBe("Value cannot be null. (Parameter 'interfaceType')");
        }

        [Fact]
        public void AddThrowsExceptionNullConcreteType()
        {
            var exception = Should.Throw<ArgumentNullException>(() => _factory.Add(typeof(ITestClass), null));
            
            exception.Message.ShouldBe("Value cannot be null. (Parameter 'instanceType')");
        }

        [Fact]
        public void AddThrowsExceptionForInvalidInterfaceType()
        {
            var exception = Should.Throw<ArgumentException>(() => _factory.Add<TestClass, TestClass>());
            
            exception.Message.ShouldBe("Type is not interface. (Parameter 'interfaceType')");
        }

        [Fact]
        public void AddThrowsExceptionForInvalidConcreteType()
        {
            var exception = Should.Throw<ArgumentException>(() => _factory.Add<ITestClass, ITestClass>());
            
            exception.Message.ShouldBe("Instance is not a concrete type. (Parameter 'instanceType')");
        }

        [Fact]
        public void AddRegistersSelf()
        {
            var resolver = _factory.Get<IFactoryResolver>();

            resolver.ShouldNotBeNull();
        }

        [Fact]
        public void AddRegistersType()
        {
            var item = _factory.Add<ITestClass, TestClass>();

            item.InstanceTypes.ContainsKey(typeof(TestClass).FullName).ShouldBeTrue();
        }

        [Fact]
        public void AddRegistersTypeWithKey()
        {
            var item = _factory.Add<ITestClass, TestClass>("test-class");

            item.InstanceTypes.ContainsKey("test-class").ShouldBeTrue();
        }

        [Fact]
        public void AddRegistersTypeAsSingleton()
        {
            var item = _factory.Add<ITestClass, TestClass>();

            item.AsSingleton();

            var manager = item.InstanceTypes[typeof(TestClass).FullName].LifetimeManager;
            manager.ShouldBeOfType<SingletonLifetimeManager>();
        }
        
        [Fact]
        public void GetReturnsInstance()
        {
            var data = new[] {null, "", " "};

            foreach (var key in data)
            {
                _factory.Add<ITestClass, TestClass>(key);

                var instance = _factory.Get<ITestClass>();

                instance.ShouldBeOfType<TestClass>();
            }
        }

        [Fact]
        public void GetReturnsInstanceFromAbstractDefinition()
        {
            var data = new[] { null, "", " " };

            foreach (var key in data)
            {
                _factory.Add<ITestClass, YetAnotherTestClass>();
                _factory.Add<AbstractTestClass, TestClass2>(key);

                var instance = _factory.Get<ITestClass>();

                instance.ShouldBeOfType<YetAnotherTestClass>();
            }
        }

        [Fact]
        public void GetReturnsInstanceWithKey()
        {
            _factory.Add<ITestClass, TestClass>("test-class");

            var instance = _factory.Get(typeof(ITestClass), "test-class");
            instance.ShouldBeOfType<TestClass>();
        }

        [Fact]
        public void GetReturnsSingletonInstance()
        {
            _factory.Add<ITestClass, TestClass>().AsSingleton();
            var instance1 = _factory.Get(typeof(ITestClass));
            
            var instance2 = _factory.Get(typeof(ITestClass));
            
            instance1.ShouldBe(instance2);
        }

        [Fact]
        public void GetReturnsTransientInstance()
        {
            _factory.Add<ITestClass, TestClass>();

            var instance1 = _factory.Get(typeof(ITestClass));
            var instance2 = _factory.Get(typeof(ITestClass));

            instance1.ShouldNotBe(instance2);
        }

        [Fact]
        public void GetReturnsNullInstanceWithUnknownKey()
        {
            _factory.Add<ITestClass, TestClass>("test-class");

            var instance = _factory.Get(typeof(ITestClass), "unknown-test-class");

            instance.ShouldBeNull();
        }

        [Fact]
        public void GetAllReturnsAllInstances()
        {
            _factory.Add<ITestClass, TestClass>();
            _factory.Add<ITestClass, AnotherTestClass>();

            var instances = _factory.GetAll<ITestClass>();

            instances.Any(i => i.GetType() == typeof(TestClass)).ShouldBeTrue();
            instances.Any(i => i.GetType() == typeof(AnotherTestClass)).ShouldBeTrue();
        }

        [Fact]
        public void AddAllOfTypeInAssemblyReturnsExpectedCount()
        {
            var itemList = _factory.AddAll<ITestClass, FactoryTests>();

            itemList.Items.Count.ShouldBe(3);
        }

        [Fact]
        public void AddAllOfTypeInAssemblyReturnsDefaultTransient()
        {
            var itemList = _factory.AddAll<ITestClass, FactoryTests>();

            itemList.Items.ForEach(i => i.CurrentLifetimeManager.ShouldBeOfType<TransientLifetimeManager>());
        }

        [Fact]
        public void AddAllOfTypeInAssemblyReturnsSingletonRegistration()
        {
            var itemList = _factory.AddAll<ITestClass, FactoryTests>();

            itemList.AsSingleton();
            
            itemList.Items.ForEach(i => i.CurrentLifetimeManager.ShouldBeOfType<SingletonLifetimeManager>());
        }

        [Fact]
        public void LogWritesToTestLogger()
        {
            _factory.AddAll<ITestClass, FactoryTests>();
            var logger = new TestConfigLogger();

            _factory.Log(logger);

            logger.Message.ShouldNotBeNull();
        }
    }
}
