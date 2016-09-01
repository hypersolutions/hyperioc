using System;

namespace HyperIoC
{
    /// <summary>
    /// Builds a factory with registered items from a set of profiles.
    /// </summary>
    public class FactoryBuilder
    {
        private readonly Factory _factory;

        private FactoryBuilder(Factory factory = null)
        {
            _factory = factory ?? new Factory();
        }

        /// <summary>
        /// Begins the factory creation.
        /// </summary>
        /// <param name="factory">Create a builder from another factory</param>
        /// <returns>Factory builder</returns>
        public static FactoryBuilder Build(Factory factory = null)
        {
            return new FactoryBuilder(factory);
        }

        /// <summary>
        /// Adds a profile if the condition is met.
        /// </summary>
        /// <param name="canAdd">Optional can add predicate</param>
        /// <returns>Factory builder</returns>
        public FactoryBuilder WithProfile<TProfile>(Func<bool> canAdd = null) where TProfile : FactoryProfile, new ()
        {
            if (canAdd == null || canAdd())
            {
                var profile = new TProfile();
                profile.Construct(_factory);
            }

            return this;
        }

        /// <summary>
        /// Creates the factory.
        /// </summary>
        /// <returns>Factory with registered items</returns>
        public Factory Create()
        {
            return _factory;
        }
    }
}
