using System;

namespace HyperIoC
{
    /// <summary>
    /// Defines the members of the IFactoryBuilder interface.
    /// </summary>
    public interface IFactoryBuilder
    {
        /// <summary>
        /// Adds an item into the IoC.
        /// </summary>
        /// <typeparam name="TInterface">Interface to register</typeparam>
        /// <typeparam name="TType">Type to register with interface</typeparam>
        /// <param name="key">Optional key</param>
        /// <returns>Register item</returns>
        Item Add<TInterface, TType>(string key = null) where TType : TInterface;

        /// <summary>
        /// Adds an item into the IoC.
        /// </summary>
        /// <param name="interfaceType">Interface to register</param>
        /// <param name="instanceType">Type to register with interface</param>
        /// <param name="key">Optional key</param>
        /// <returns>Register item</returns>
        Item Add(Type interfaceType, Type instanceType, string key = null);

        /// <summary>
        /// Adds an item into the IoC.
        /// </summary>
        /// <typeparam name="TInterface">Interface to register</typeparam>
        /// <typeparam name="TAssemblyType">Type in assembly to discover TInterface implementations</typeparam>
        /// <returns>Register item list</returns>
        ItemList AddAll<TInterface, TAssemblyType>();
    }
}