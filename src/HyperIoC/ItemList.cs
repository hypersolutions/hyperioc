using System.Collections.Generic;
using HyperIoC.Lifetime;

namespace HyperIoC
{
    /// <summary>
    /// Provides a wrapper around a list of registered items implementing the same interface.
    /// </summary>
    public class ItemList
    {
        internal ItemList()
        {
            Items= new List<Item>();
        }
        
        internal List<Item> Items { get; }

        /// <summary>
        /// Signals that this item should be registered as a singleton.
        /// </summary>
        public void AsSingleton()
        {
            Items.ForEach(i => i.AsSingleton());
        }

        /// <summary>
        /// Allows specifying a custom lifetime manager for your objects.
        /// </summary>
        /// <typeparam name="TLifetimeManager">Lifetime manager type</typeparam>
        public void SetLifetimeTo<TLifetimeManager>() where TLifetimeManager : ILifetimeManager, new ()
        {
            Items.ForEach(i => i.SetLifetimeTo(new TLifetimeManager()));
        }
    }
}
