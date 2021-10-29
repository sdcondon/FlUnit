using System;
using System.Runtime.ExceptionServices;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    /// <typeparam name="T">The return type of test function.</typeparam>
    internal sealed class TestFunctionOutcome<T>
    {
        internal TestFunctionOutcome(T result) => Result = result;

        internal TestFunctionOutcome(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets the return value of the when clause, as long as an exception was not thrown.
        /// </summary>
        /// <remarks>
        /// TODO: this should probably throw the exception if there is one - like Task does? Don't want to mess with the stack trace though.. Throw invalidop with an inner?
        /// </remarks>
        public T Result { get; }

        /// <summary>
        /// Gets the exception that was thrown, or null.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Throws the exception (preserving stack trace) if it is populated.
        /// </summary>
        public void ThrowIfException()
        {
            if (Exception != null)
            {
                ExceptionDispatchInfo.Capture(Exception).Throw();
            }
        }

        /// <summary>
        /// Throws an exception if the outcome indicates that the test action returned successfully.
        /// </summary>
        public void ThrowIfNoException()
        {
            if (Exception == null)
            {
                // TODO-MAINTAINABILITY: Ick, new Exception.. Some kind of FlUnit-specific Assert.Fail and exception types would be good..
                // TODO-LOCALISATION: Localisation needed if this ever catches on
                throw new Exception("An exception was expected but the When clause didn't throw one");
            }
        }
    }
}
