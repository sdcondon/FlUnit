﻿namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface that should be implemented by test adapters, and an instance thereof passed to <see cref="TestRun{TTestDesriptor}"/> instances, for
    /// them to use to communicate the results of a test run back to the test runner.
    /// </summary>
    /// <typeparam name="TTestDescriptor">The type of test descriptor that will be passed to this class.</typeparam>
    /// <remarks>
    /// TODO: A design where the result is communicated via methods on the  test descriptor itself is also worth
    /// considering, and would eliminate the need for generics altogether. The question is if we're going to need to report
    /// things about the run as a whole..
    /// </remarks>
    internal interface ITestRunResultHandler<TTestDescriptor>
    {
        /// <summary>
        /// Request an instance for handling the results of an individual test.
        /// </summary>
        /// <param name="testDescriptor">The descriptor of the test to create a handler for the results of.</param>
        /// <returns>A <see cref="ITestResultHandler"/> instance.</returns>
        ITestResultHandler CreateTestResultHandler(TTestDescriptor testDescriptor);
    }
}