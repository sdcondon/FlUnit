using System;

namespace FlUnit
{
    /// <summary>
    /// Container for the outcome of the invoking the "When" clause of a test.
    /// </summary>
    /// <typeparam name="T">The return type of test function.</typeparam>
    public sealed class TestFunctionOutcome<T>
    {
        private readonly T result;

        internal TestFunctionOutcome(T result) => this.result = result;

        internal TestFunctionOutcome(Exception exception) => Exception = exception;

        /// <summary>
        /// Gets the return value of the when clause, as long as an exception was not thrown.
        /// </summary>
        public T Result
        {
            get
            {
                if (Exception != null)
                {
                    throw new InvalidOperationException("When clause threw an exception", Exception);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the exception that was thrown, or null.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Throws a <see cref="TestFailureException"/> if the Exception property is populated.
        /// </summary>
        public void ThrowIfException()
        {
            if (Exception != null)
            {
                throw new TestFailureException(string.Format(Messages.TestOutcomeExceptionNotExpectedButThrownFormat, Exception.Message), Exception.StackTrace, Exception);
            }
        }

        /// <summary>
        /// Throws a <see cref="TestFailureException"/> if the outcome indicates that the test action returned successfully.
        /// </summary>
        public void ThrowIfNoException()
        {
            if (Exception == null)
            {
                throw new TestFailureException(Messages.TestOutcomeExceptionExpectedButNotThrown);
            }
        }
    }
}
