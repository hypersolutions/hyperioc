namespace HyperIoC
{
    /// <summary>
    /// Base class for building profiles.
    /// </summary>
    public abstract class FactoryProfile
    {
        /// <summary>
        /// Constructs the IoC configuration.
        /// </summary>
        /// <param name="builder">Builder to attach to</param>
        public abstract void Construct(IFactoryBuilder builder);
    }
}
