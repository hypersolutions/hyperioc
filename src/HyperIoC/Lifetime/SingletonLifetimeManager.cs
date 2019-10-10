using System;

namespace HyperIoC.Lifetime
{
    /// <summary>
    /// Defines a singleton lifetime scope.
    /// </summary>
    public class SingletonLifetimeManager : LifetimeManager
    {
        private volatile object _instance;
        private readonly object _syncRoot = new object();

        /// <summary>
        /// Initialises a new instance of the class.
        /// </summary>
        /// <param name="instance">Instance to use</param>
        public SingletonLifetimeManager(object instance = null)
        {
            _instance = instance;
        }

        public override object Get(Type type, IFactoryLocator locator, IFactoryResolver resolver)
        {
            if (_instance != null) return _instance;

            lock (_syncRoot)
            {
                if (_instance == null)
                {
                    _instance = CreateInstance(type, locator, resolver);
                }
            }

            return _instance;
        }
    }
}
