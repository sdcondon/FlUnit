using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    /// <typeparam name="T">The return type of test action.</typeparam>
    public class TestActionResult
    {
        internal TestActionResult() { }

        internal TestActionResult(Exception exception) => Exception = exception;

        public Exception Exception { get; }
    }
}