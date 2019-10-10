using System;

namespace HyperIoC.Lifetime
{
    /// <summary>
    /// Defines a transient lifetime scope.
    /// </summary>
    public class TransientLifetimeManager : LifetimeManager
    {
        public override object Get(Type type, IFactoryLocator locator, IFactoryResolver resolver)
        {
            return CreateInstance(type, locator, resolver);
        }
    }
}
