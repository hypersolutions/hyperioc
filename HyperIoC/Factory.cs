using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if WINDOWS_UWP
using System.Reflection;
#endif
using HyperIoC.Lifetime;
using HyperIoC.Logging;

namespace HyperIoC
{
    /// <summary>
    /// Provides registration of items in the factory for resolving via interfaces and abstract classes.
    /// </summary>
    public class Factory : IFactoryBuilder, IFactoryResolver, IFactoryLocator
    {
        private readonly List<Item> _items = new List<Item>();

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public Factory()
        {
            // Add self to the IoC (as resolver) for dependency resolution patterns.
            var item = new Item(typeof (IFactoryResolver));
            item.AddType(GetType().FullName, GetType());
            item.SetLifetimeTo(new SingletonLifetimeManager(this));
            _items.Add(item);
        }

        private IFactoryLocator Locator => this;

        /// <summary>
        /// Adds an item into the IoC.
        /// </summary>
        /// <typeparam name="TInterface">Interface to register</typeparam>
        /// <typeparam name="TType">Type to register with interface</typeparam>
        /// <param name="key">Optional key</param>
        /// <returns>Register item</returns>
        public Item Add<TInterface, TType>(string key = null) where TType : TInterface
        {
            return Add(typeof (TInterface), typeof (TType), key);
        }

        /// <summary>
        /// Adds an item into the IoC.
        /// </summary>
        /// <param name="interfaceType">Interface to register</param>
        /// <param name="instanceType">Type to register with interface</param>
        /// <param name="key">Optional key</param>
        /// <returns>Register item</returns>
        public Item Add(Type interfaceType, Type instanceType, string key = null)
        {
            CheckInterfaceType(interfaceType);
            CheckInstanceType(instanceType);

            var item = Locator.FindItem(interfaceType);

            key = key ?? instanceType.FullName;

            if (item == null) _items.Add(new Item(interfaceType));
            
            item = Locator.FindItem(interfaceType);

            if (!item.InstanceTypes.ContainsKey(key))
            {
                item.AddType(key, instanceType);
            }

            return item;
        }

        /// <summary>
        /// Adds an item into the IoC.
        /// </summary>
        /// <typeparam name="TInterface">Interface to register</typeparam>
        /// <typeparam name="TAssemblyType">Type in assembly to discover TInterface implementations</typeparam>
        /// <returns>Register item list</returns>
        public ItemList AddAll<TInterface, TAssemblyType>()
        {
            var interfaceType = typeof(TInterface);

            CheckInterfaceType(interfaceType);

            var items = new ItemList();

#if WINDOWS_UWP
            var asm = typeof(TAssemblyType).GetTypeInfo().Assembly;
#else
            var asm = typeof(TAssemblyType).Assembly;
#endif
            foreach (var type in asm.GetTypes().Where(t => interfaceType.IsAssignableFrom(t)))
            {
                if (!CanAddInstanceType(type)) continue;

                var item = Add(interfaceType, type);
                items.Items.Add(item);
            }

            return items;
        }

        /// <summary>
        /// Gets an item from the IoC for the requesting interface.
        /// </summary>
        /// <typeparam name="TInterface">Interface to resolve type for</typeparam>
        /// <param name="key">Optional key</param>
        /// <returns>Instance if found else null</returns>
        public TInterface Get<TInterface>(string key = null)
        {
            var interfaceType = typeof(TInterface);
            return (TInterface)Get(interfaceType, key);
        }

        /// <summary>
        /// Gets an item from the IoC for the requesting interface.
        /// </summary>
        /// <param name="interfaceType">Interface to resolve type for</param>
        /// <param name="key">Optional key</param>
        /// <returns>Instance if found else null</returns>
        public object Get(Type interfaceType, string key = null)
        {
            CheckInterfaceType(interfaceType);

            var item = Locator.FindItem(interfaceType);

            if (item == null) return null;

            // If no key supplied then use the first one
            if (string.IsNullOrWhiteSpace(key))
            {
                key = item.InstanceTypes.Keys.First();
            }

            // If we have a type that matches the key then use this else return null
            return item.InstanceTypes.ContainsKey(key)
                ? item.InstanceTypes[key].LifetimeManager.Get(item.InstanceTypes[key].Type, this, this)
                : null;
        }

        /// <summary>
        /// Gets all the instances of the interface from the IoC.
        /// </summary>
        /// <param name="interfaceType">Interface to resolve types for</param>
        /// <returns>Resolved instances</returns>
        public object[] GetAll(Type interfaceType)
        {
            CheckInterfaceType(interfaceType);

            var item = Locator.FindItem(interfaceType);

            if (item == null) return null;

            var instanceTypes = new List<object>();

            foreach (var detail in item.InstanceTypes.Values)
            {
                var instance = detail.LifetimeManager.Get(detail.Type, this, this);
                instanceTypes.Add(instance);
            }

            return instanceTypes.ToArray();
        }

        /// <summary>
        /// Gets all the instances of the interface from the IoC.
        /// </summary>
        /// <typeparam name="TInterface">Interface to resolve type for</typeparam>
        /// <returns>Resolved instances</returns>
        public TInterface[] GetAll<TInterface>()
        {
            var instances = GetAll(typeof (TInterface)).ToList();
            var castInstances = new List<TInterface>();
            instances.ForEach(i => castInstances.Add((TInterface)i));
            return castInstances.ToArray();
        }

        /// <summary>
        /// Logs the configured registration.
        /// </summary>
        /// <param name="logger">Logger to write to. If null then the debug logger will be used.</param>
        public void Log(IConfigLogger logger = null)
        {
            var config = new StringBuilder();

            config.AppendLine("Logging the registration...");
            config.AppendLine();

            foreach (var item in _items)
            {
                config.AppendFormat("Registered type: '{0}' contains...", item.InterfaceType.FullName);
                config.AppendLine();

                foreach (var instanceType in item.InstanceTypes)
                {
                    config.AppendFormat("Type: '{0}' with key '{1}' as '{2}' lifetime", 
                        instanceType.Value.Type, instanceType.Key, instanceType.Value.LifetimeManager);
                    config.AppendLine();
                }

                config.AppendFormat("Registered type: '{0}' complete.", item.InterfaceType.FullName);
                config.AppendLine();
                config.AppendLine();
            }

            config.AppendLine("Registration log complete.");

            var theLogger = logger ?? new DebugConfigLogger();
            theLogger.Log(config.ToString());
        }

        /// <summary>
        /// Locates an item in the IoC registration list.
        /// </summary>
        /// <param name="interfaceType">Interface type to find</param>
        /// <returns>Item if found else null</returns>
        Item IFactoryLocator.FindItem(Type interfaceType)
        {
            return _items.FirstOrDefault(i => i.InterfaceType == interfaceType);
        }
        
        private static void CheckInterfaceType(Type interfaceType)
        {
            if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));

#if WINDOWS_UWP
            if (!(interfaceType.GetTypeInfo().IsInterface || interfaceType.GetTypeInfo().IsAbstract))
                throw new ArgumentException("Type is not interface.", nameof(interfaceType));
#else
            if (!(interfaceType.IsInterface || interfaceType.IsAbstract))
                throw new ArgumentException("Type is not interface.", nameof(interfaceType));
#endif
        }

        private static void CheckInstanceType(Type instanceType)
        {
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));

#if WINDOWS_UWP
            if (instanceType.GetTypeInfo().IsInterface || instanceType.GetTypeInfo().IsAbstract)
                throw new ArgumentException("Instance is not a concerete type.", nameof(instanceType));
#else
            if (instanceType.IsInterface || instanceType.IsAbstract)
                throw new ArgumentException("Instance is not a concerete type.", nameof(instanceType));
#endif
        }

        private static bool CanAddInstanceType(Type instanceType)
        {
            if (instanceType == null) return false;

#if WINDOWS_UWP
            if (instanceType.GetTypeInfo().IsInterface || instanceType.GetTypeInfo().IsAbstract)
                return false;
#else
            if (instanceType.IsInterface || instanceType.IsAbstract)
                return false;
#endif
            return true;
        }
    }
}