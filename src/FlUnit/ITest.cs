using System;
using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Interface for types representing a runnable test.
    /// </summary>
    /// <remarks>
    /// NB: No arrange method. The way it's desgined so far, arrangement happens as the test itself is built.. Makes for a few fewer delegates when building tests, but not sure I like this..
    /// </remarks>
    public interface ITest
    {
        /// <summary>
        /// Invokes the test action. A thrown exception should be treated as test failure.
        /// </summary>
        void Act();

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        IEnumerable<TestAssertion> Assertions { get; }
    }
}
