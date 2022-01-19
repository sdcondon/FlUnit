using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// The VSTest adapter's implementation of <see cref="ITestRunResultHandler{TestDescriptor}"/>.
    /// Intended for consumption by FlUnit's core execution logic to pass test run results back to the VSTest platform.
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
