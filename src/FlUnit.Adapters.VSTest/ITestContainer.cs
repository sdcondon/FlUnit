﻿using System;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface for test containers passed to the core execution logic (i.e. <see cref="TestRun"/> instances) by adapters.
    /// Includes the <see cref="FlUnit.TestMetadata"/> that enables the test to be run, as well as logic for reporting its results back to the test runner.
    /// </summary>
    /// <remarks>
    /// NB: This interface worries me a little - the methods look far too influenced by VSTest.
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
        /// TODO: I don't like this method. Its far too close to the VSTest platform (inc all of the assumptions made re what test
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
