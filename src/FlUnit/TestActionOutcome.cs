using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    public sealed class TestActionOutcome
    {
        internal TestActionOutcome() { }

        internal TestActionOutcome(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets the exception that was thrown, or null.
        /// </summary>
        public Exception Exception { get; }
    }
}