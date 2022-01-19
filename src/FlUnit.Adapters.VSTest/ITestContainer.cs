using System;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface for test containers that include both the <see cref="TestMetadata"/> that enables core execution
    /// logic (i.e. <see cref="TestRun"/> instances) to run a test, and logic for reporting its results back to the test runner.
    /// </summary>

    internal interface ITestContainer
    {
        /// <summary>
        /// Gets the FlUnit metadata for this test (which FlUnit execution logic can use to run it).
        /// </summary>
        TestMetadata TestMetadata { get; }

        /// <summary>
        /// Records that execution of this test has begun.
        /// </summary>
        void RecordStart();

        /// <remarks>
        /// I don't like this method. Its far too close to the VSTest platform (inc all of the assumptions made re what test
        /// result pluralilty means, and how display names are interpreted). Better to talk in FlUnit terms and expect the adapter do
        /// more..
        /// </remarks>
        void RecordResult(
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string displayName,
            TestOutcome outcome,
            string errorMessage,
            string errorStackTrace);

        /// <summary>
        /// Records that execution of this test has ended.
        /// </summary>
        void RecordEnd(TestOutcome outcome);
    }
}
