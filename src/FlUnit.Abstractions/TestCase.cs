using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing an individual test case.
    /// </summary>
    /// <remarks>
    /// NB: This is an abstract class rather than an interface to facilitate implicit conversions from builders.
    /// </remarks>
    public abstract class TestCase
    {
        /// <summary>
        /// Gets the description of this case.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Invokes the test action.
        /// </summary>
        public abstract void Act();

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public abstract IReadOnlyCollection<TestAssertion> Assertions { get; }
    }
}
