using System;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface that should be implemented by test adapters for communicating the results of an individual test to the runner.
    /// </summary>
    internal interface ITestResultHandler
    {
        void RecordStart();

        void RecordResult(
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string displayName,
            TestOutcome outcome,
            string errorMessage,
            string errorStackTrace);

        void RecordEnd(
            TestOutcome outcome);
    }
}
