using System;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface for test containers passed to the core execution logic (i.e. <see cref="TestRun"/> instances) by adapters.
    /// Includes the <see cref="FlUnit.TestMetadata"/> that enables the <see cref="TestRun"/> to run the test, as well as the API for reporting its results back to the test runner.
    /// </summary>
    /// <remarks>
    /// NB: This interface worries me a little - the method signatures are far too influenced by VSTest.
    /// It will stay internal until such time as a second adapter exists, to limit the impact of said adapter prompting changes..
    /// <para/>
    /// The other slight worry here is that its going to be too large and unfocused - especially if I end up adding Log methods to it.
    /// Originally, I had separate metadata container, ITestRunResultHandler and ITestResultHandler - may return to that..
    /// </remarks>
    internal interface ITestContainer
    {
        /// <summary>
        /// Gets the FlUnit metadata for this test.
        /// </summary>
        TestMetadata TestMetadata { get; }

        /// <summary>
        /// Records that execution of this test has begun.
        /// </summary>
        void RecordStart();

        /// <remarks>
        /// TODO-MAINTAINABILITY: I don't like this method. Its far too close to the VSTest platform (inc all of the assumptions made re what test
        /// result pluralilty means, and how display names are interpreted). Better to talk in FlUnit terms and expect the adapter do
        /// more (e.g. RecordCase, RecordAssertion, or similar)..
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
