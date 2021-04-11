using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    /// <typeparam name="T">The return type of test function.</typeparam>
    public sealed class TestFunctionOutcome<T>
    {
        internal TestFunctionOutcome(T result) => Result = result;

        internal TestFunctionOutcome(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets the return value of the when clause, as long as an exception was not thrown.
        /// <para/>
        /// TODO: this should probably throw the exception if there is one - like Task does? Don't want to mess with the stack trace though.. Throw invalidop with an inner?
        /// </summary>
        public T Result { get; }

        /// <summary>
        /// Gets the exception that was thrown, or null.
        /// </summary>
        public Exception Exception { get; }
    }
}
