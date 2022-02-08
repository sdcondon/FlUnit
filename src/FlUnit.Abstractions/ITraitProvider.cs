using System.Reflection;

namespace FlUnit
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
        /// <param name="testProperty">Information about the test property to get the trait for.</param>
        /// <returns>The trait to apply to the test.</returns>
        ITrait GetTrait(PropertyInfo testProperty);
    }
}
