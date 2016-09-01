using System;
using HyperIoC.Lifetime;

namespace HyperIoC
{
    /// <summary>
    /// Defines the details of an IoC item.
    /// </summary>
    public class ItemDetail
    {
        internal ItemDetail(Type type)
        {
            Type = type;
            LifetimeManager = new TransientLifetimeManager();
        }

        internal Type Type { get; private set; }
        internal ILifetimeManager LifetimeManager { get; set; }
    }
}