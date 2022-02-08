using System;

namespace FlUnit
{
    /// <summary>
    /// Interface for <see cref="Exception"/> classes thrown by <see cref="Test"/> instances to
    /// implement to communicate error details to the test runner.
    /// </summary>
    /// <remarks>
    /// Platform adapters should handle any thrown exception by recording a test failure result,
    /// but using this interface gives test implementations finer control over the test error
    /// message and stack trace (for example, to omit framework frames from the stack traces).
    /// </remarks>
    public interface ITestFailureDetails
    {
        /// <summary>
        /// Gets the error message that should be recorded in the test result.
        /// </summary>
        string TestResultErrorMessage { get; }

        /// <summary>
        /// Gets the error stack trace that should be recorded in the test result.
        /// </summary>
        string TestResultErrorStackTrace { get; }
    }
}
