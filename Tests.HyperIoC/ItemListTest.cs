using HyperIoC;
using HyperIoC.Lifetime;
#if WINDOWS_UWP
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using Tests.HyperIoC.Support;

namespace Tests.HyperIoC
{
    [TestClass]
    public class ItemListTest
    {
        private ItemList _itemList;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _itemList = new ItemList();
        }

        [TestMethod]
        public void DefaultConfiguresAllItemsAsDefaultLifetime()
        {
            var item = new Item(typeof(ITestClass));
            item.AddType("", typeof(TestClass));
            _itemList.Items.Add(item);
            item = new Item(typeof(ITestClass));
            item.AddType("", typeof(AnotherTestClass));
            _itemList.Items.Add(item);

            _itemList.Items.ForEach(i => Assert.IsInstanceOfType(
                i.CurrentLifetimeManager, typeof(TransientLifetimeManager)));
        }

        [TestMethod]
        public void AsSingletonConfiguresAllItemsAsSameLifetime()
        {
            var item = new Item(typeof(ITestClass));
            item.AddType("", typeof(TestClass));
            _itemList.Items.Add(item);
            item = new Item(typeof(ITestClass));
            item.AddType("", typeof(AnotherTestClass));
            _itemList.Items.Add(item);

            _itemList.AsSingleton();

            _itemList.Items.ForEach(i => Assert.IsInstanceOfType(
                i.CurrentLifetimeManager, typeof(SingletonLifetimeManager)));
        }

        [TestMethod]
        public void SetLifetimeToConfiguresAllItemsAsSameLifetime()
        {
            var item = new Item(typeof(ITestClass));
            item.AddType("", typeof(TestClass));
            _itemList.Items.Add(item);
            item = new Item(typeof(ITestClass));
            item.AddType("", typeof(AnotherTestClass));
            _itemList.Items.Add(item);

            _itemList.SetLifetimeTo<TestLifetimeManager>();

            _itemList.Items.ForEach(i => Assert.IsInstanceOfType(
                i.CurrentLifetimeManager, typeof(TestLifetimeManager)));
        }
    }
}