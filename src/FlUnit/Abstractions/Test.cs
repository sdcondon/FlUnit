using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing a runnable test.
    /// </summary>
    public abstract class Test
    {
        /// <summary>
        /// Arranges the test.
        /// </summary>
        public abstract void Arrange();

        /// <summary>
        /// Invokes the test action.
        /// </summary>
        public abstract void Act();

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public abstract IEnumerable<TestAssertion> Assertions { get; }
    }
}
