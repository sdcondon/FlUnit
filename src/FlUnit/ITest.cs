using System;
using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Interface for types representing a runnable test.
    /// </summary>
    public interface ITest
    {
        void Run();

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Arrange"/> and <see cref="Run"/> have been invoked.
        /// </summary>
        IEnumerable<TestAssertion> Assertions { get; }
    }
}
