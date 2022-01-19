using System;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface that should be implemented by test adapters for communicating the results of an individual test to the runner.
    /// </summary>
    /// <remarks>
    /// I don't like this interface. Its far too close to the VSTest platform (inc all of the assumptions made re what test
    /// result pluralilty means, and how display names are interpreted). Better to talk in FlUnit terms and have the adapter do
    /// more..
    /// </remarks>
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

        void RecordEnd(TestOutcome outcome);
    }
}
