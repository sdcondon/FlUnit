using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing a runnable test.
    /// <para/>
    /// NB: No arrange method. The way it's designed so far, arrangement happens as the test itself is built. Makes for a fewer delegates when building tests, but not sure I like this..
    /// </summary>
    public abstract class Test
    {
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
