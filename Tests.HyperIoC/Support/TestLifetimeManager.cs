using System;
using HyperIoC;
using HyperIoC.Lifetime;

namespace Tests.HyperIoC.Support
{
    public class TestLifetimeManager : ILifetimeManager
    {
        public object Get(Type type, IFactoryLocator locator, IFactoryResolver resolver)
        {
            throw new NotImplementedException();
        }
    }
}
