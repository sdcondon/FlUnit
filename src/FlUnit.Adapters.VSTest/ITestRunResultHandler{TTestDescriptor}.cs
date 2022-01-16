namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface that should be implemented by test adapters for communicating the results of a test run to the runner.
    /// </summary>
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
