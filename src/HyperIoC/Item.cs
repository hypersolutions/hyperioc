using System;
using System.Collections.Generic;
using HyperIoC.Lifetime;

namespace HyperIoC
{
    /// <summary>
    /// Defines a IoC registered item. All items are registered as transient by default.
    /// </summary>
    public class Item
    {
        private ItemDetail _currentTypeDetail;

        internal Item(Type interfaceType)
        {
            InstanceTypes = new Dictionary<string, ItemDetail>();
            InterfaceType = interfaceType;
        }

        internal Type InterfaceType { get; }

        internal Dictionary<string, ItemDetail> InstanceTypes { get; }

        internal ILifetimeManager CurrentLifetimeManager => _currentTypeDetail.LifetimeManager;

        /// <summary>
        /// Signals that this item should be registered as a singleton.
        /// </summary>
        public void AsSingleton()
        {
            _currentTypeDetail.LifetimeManager = new SingletonLifetimeManager();
        }

        /// <summary>
        /// Allows specifying a custom lifetime manager for your objects.
        /// </summary>
        /// <param name="lifetimeManager">Lifetime manager</param>
        public void SetLifetimeTo(ILifetimeManager lifetimeManager)
        {
            if (_currentTypeDetail != null && lifetimeManager != null)
                _currentTypeDetail.LifetimeManager = lifetimeManager;
        }

        internal void AddType(string key, Type type)
        {
            _currentTypeDetail = new ItemDetail(type);
            InstanceTypes.Add(key, _currentTypeDetail);
        }
    }
}
