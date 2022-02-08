using System;

namespace FlUnit
{
    /// <summary>
    /// Exception class thrown to indicate test failure, with fine control over the error details in the test result.
    /// </summary>
    public class TestFailureException : Exception, ITestFailureDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestFailureException"/> class.
        /// </summary>
        /// <param name="testResultErrorMessage">The error message for the test result.</param>
        /// <param name="testResultErrorStackTrace">The stack trace for the test result.</param>
        /// <param name="innerException">The exception that prompted this failure.</param>
        public TestFailureException(string testResultErrorMessage, string testResultErrorStackTrace, Exception innerException)
            : base($"FlUnit test failed with error message: {testResultErrorMessage}", innerException)
        {
            TestResultErrorMessage = testResultErrorMessage;
            TestResultErrorStackTrace = testResultErrorStackTrace;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestFailureException"/> class.
        /// </summary>
        /// <param name="testResultErrorMessage">The error message for the test result.</param>
        public TestFailureException(string testResultErrorMessage)
            : base($"FlUnit test failed with error message: {testResultErrorMessage}")
        {
            TestResultErrorMessage = testResultErrorMessage;
        }

        /// <summary>
        /// The error message that should be recorded in the test result.
        /// </summary>
        public string TestResultErrorMessage { get; }

        /// <summary>
        /// The error stack trace that should be recorded in the test result.
        /// </summary>
        public string TestResultErrorStackTrace { get; }
    }
}
