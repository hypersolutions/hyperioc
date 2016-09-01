using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HyperIoC.Lifetime
{
    /// <summary>
    /// Lifetime manager base class.
    /// </summary>
    public abstract class LifetimeManager : ILifetimeManager
    {
        public abstract object Get(Type type, IFactoryLocator locator, IFactoryResolver resolver);

        protected object CreateInstance(Type type, IFactoryLocator locator, IFactoryResolver resolver)
        {
            // Multple constructors are NOT a good design pattern. Take the first one found.
            var ctor = type.GetConstructors().First();
            var ctorParams = new List<object>();

            foreach (var paramInfo in ctor.GetParameters())
            {
#if WINDOWS_UWP
                if (!(paramInfo.ParameterType.GetTypeInfo().IsInterface ||
                    paramInfo.ParameterType.GetTypeInfo().IsAbstract))
                {
                    ctorParams.Clear();
                    break;
                }
#else
                if (!(paramInfo.ParameterType.IsInterface || paramInfo.ParameterType.IsAbstract))
                {
                    ctorParams.Clear();
                    break;
                }
#endif
                var item = locator.FindItem(paramInfo.ParameterType);

                if (item == null)
                {
                    ctorParams.Clear();
                    break;
                }

                var instance = resolver.Get(paramInfo.ParameterType);
                ctorParams.Add(instance);
            }

            return Activator.CreateInstance(type, ctorParams.ToArray());
        }
    }
}