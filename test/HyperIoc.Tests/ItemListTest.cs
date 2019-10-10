using HyperIoC.Lifetime;
using HyperIoC.Tests.Support;
using Shouldly;
using Xunit;

namespace HyperIoC.Tests
{
    public class ItemListTest
    {
        private ItemList _itemList;

        public ItemListTest()
        {
            _itemList = new ItemList();
        }

        [Fact]
        public void DefaultConfiguresAllItemsAsDefaultLifetime()
        {
            var item = new Item(typeof(ITestClass));
            item.AddType("", typeof(TestClass));
            _itemList.Items.Add(item);
            item = new Item(typeof(ITestClass));
            item.AddType("", typeof(AnotherTestClass));
            _itemList.Items.Add(item);

            _itemList.Items.ForEach(i => i.CurrentLifetimeManager.ShouldBeOfType<TransientLifetimeManager>());
        }

        [Fact]
        public void AsSingletonConfiguresAllItemsAsSameLifetime()
        {
            var item = new Item(typeof(ITestClass));
            item.AddType("", typeof(TestClass));
            _itemList.Items.Add(item);
            item = new Item(typeof(ITestClass));
            item.AddType("", typeof(AnotherTestClass));
            _itemList.Items.Add(item);

            _itemList.AsSingleton();

            _itemList.Items.ForEach(i => i.CurrentLifetimeManager.ShouldBeOfType<SingletonLifetimeManager>());
        }

        [Fact]
        public void SetLifetimeToConfiguresAllItemsAsSameLifetime()
        {
            var item = new Item(typeof(ITestClass));
            item.AddType("", typeof(TestClass));
            _itemList.Items.Add(item);
            item = new Item(typeof(ITestClass));
            item.AddType("", typeof(AnotherTestClass));
            _itemList.Items.Add(item);

            _itemList.SetLifetimeTo<TestLifetimeManager>();

            _itemList.Items.ForEach(i => i.CurrentLifetimeManager.ShouldBeOfType<TestLifetimeManager>());
        }
    }
}
