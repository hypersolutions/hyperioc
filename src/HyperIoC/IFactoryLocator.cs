using System;

namespace HyperIoC
{
    /// <summary>
    /// Defines the members of the IFactoryLocator interface.
    /// </summary>
    public interface IFactoryLocator
    {
        Item FindItem(Type interfaceType);
    }
}
