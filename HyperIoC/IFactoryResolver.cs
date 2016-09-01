using System;

namespace HyperIoC
{
    /// <summary>
    /// Defines the members of the IFactoryResolver interface.
    /// </summary>
    public interface IFactoryResolver
    {
        /// <summary>
        /// Gets an item from the IoC for the requesting interface.
        /// </summary>
        /// <typeparam name="TInterface">Interface to resolve type for</typeparam>
        /// <param name="key">Optional key</param>
        /// <returns>Instance if found else null</returns>
        TInterface Get<TInterface>(string key = null);

        /// <summary>
        /// Gets an item from the IoC for the requesting interface.
        /// </summary>
        /// <param name="interfaceType">Interface to resolve type for</param>
        /// <param name="key">Optional key</param>
        /// <returns>Instance if found else null</returns>
        object Get(Type interfaceType, string key = null);

        /// <summary>
        /// Gets all the instances of the interface from the IoC.
        /// </summary>
        /// <param name="interfaceType">Interface to resolve types for</param>
        /// <returns>Resolved instances</returns>
        object[] GetAll(Type interfaceType);

        /// <summary>
        /// Gets all the instances of the interface from the IoC.
        /// </summary>
        /// <typeparam name="TInterface">Interface to resolve type for</typeparam>
        /// <returns>Resolved instances</returns>
        TInterface[] GetAll<TInterface>();
    }
}