using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing a runnable test.
    /// </summary>
    /// <remarks>
    /// NB: This is an abstract class rather than an interface mostly to facilitate implicit conversions from builders.
    /// </remarks>
    public abstract class Test
    {
        private static readonly Dictionary<string, string> EmptyTraitsDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Gets the traits of the test.
        /// </summary>
        /// <remarks>
        /// Default implementation returns an empty dictionary.
        /// </remarks>
        public virtual IReadOnlyDictionary<string, string> Traits { get; } = EmptyTraitsDictionary;

        /// <summary>
        /// Gets a collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        public abstract IReadOnlyCollection<ITestCase> Cases { get; }

        /// <summary>
        /// Arranges the test - populating the <see cref="Cases"/> property.
        /// </summary>
        public abstract void Arrange();
    }
}
