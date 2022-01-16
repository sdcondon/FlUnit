using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// Interface that should be implemented by test adapters for communicating the results of a test run to the runner.
    /// </summary>
    internal class TestRunResultHandler : ITestRunResultHandler<TestDescriptor>
    {
        private readonly IRunContext runContext;
        private readonly IFrameworkHandle frameworkHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRunResultHandler"/> class.
        /// </summary>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        public TestRunResultHandler(IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            this.runContext = runContext;
            this.frameworkHandle = frameworkHandle;
        }

        /// <inheritdoc />
        public ITestResultHandler CreateTestResultHandler(TestDescriptor testDescriptor)
        {
            return new TestResultHandler(runContext, frameworkHandle, testDescriptor.TestCase);
        }
    }
}
