using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Interface for types representing a runnable test.
    /// <para/>
    /// NB: No arrange method. The way it's designed so far, arrangement happens as the test itself is built. Makes for a fewer delegates when building tests, but not sure I like this..
    /// </summary>
    public interface ITest
    {
        /// <summary>
        /// Invokes the test action.
        /// </summary>
        void Act();

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        IEnumerable<TestAssertion> Assertions { get; }
    }
}
