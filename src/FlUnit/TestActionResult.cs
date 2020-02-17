using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    public class TestActionResult
    {
        internal TestActionResult() { }

        internal TestActionResult(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets the exception that was thrown, or null.
        /// </summary>
        public Exception Exception { get; }
    }
}