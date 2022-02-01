﻿namespace FlUnit
{
    /// <summary>
    /// Test runners should look for attributes attached to tests, test classes or test assemblies that implement this interface,
    /// and add the provided trait to the tests that they cover.
    /// </summary>
    public interface ITraitProvider
    {
        /// <summary>
        /// Gets the provided trait.
        /// </summary>
        /// <remarks>
        /// TODO-EXTENSIBILITY: Decide if this should be GetTrait(PropertyInfo) pre-V1.
        /// </remarks>
        Trait Trait { get; }
    }
}
