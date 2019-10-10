using System;
using HyperIoC.Lifetime;

namespace HyperIoC.Tests.Support
{
    public class TestLifetimeManager : ILifetimeManager
    {
        public object Get(Type type, IFactoryLocator locator, IFactoryResolver resolver)
        {
            throw new NotImplementedException();
        }
    }
}
