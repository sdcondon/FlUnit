using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    /// <typeparam name="T">The return type of test action.</typeparam>
    public sealed class TestFunctionResult<T>
    {
        internal TestFunctionResult(T result) => Result = result;

        internal TestFunctionResult(Exception exception) => Exception = exception;

        public T Result { get; }

        public Exception Exception { get; }
    }
}
