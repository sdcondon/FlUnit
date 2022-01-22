using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// FlUnit's implementation of <see cref="ITestExecutor"/> - takes responsibility for executing tests as discovered by <see cref="TestDiscoverer"/>.
    /// </summary>
    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private CancellationTokenSource cancellationTokenSource;

        /// <inheritdoc />
        public void RunTests(
            IEnumerable<string> sources,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            var runSettings = TestRunConfigurationReader.ReadXml(runContext.RunSettings?.SettingsXml);

            RunTests(
                TestDiscoverer.MakeTestCases(sources, runContext, null, runSettings),
                runContext,
                frameworkHandle,
                runSettings);
        }

        /// <inheritdoc />
        public void RunTests(
            IEnumerable<TestCase> tests,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            var runSettings = TestRunConfigurationReader.ReadXml(runContext.RunSettings?.SettingsXml);

            RunTests(
                tests,
                runContext,
                frameworkHandle,
                runSettings);
        }

        /// <inheritdoc />
        public void Cancel()
        {
            cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Private method for running tests once we have determined which tests we want to run and the test run configuration to use.
        /// </summary>
        /// <param name="testCases">The VSTest test cases to run.</param>
        /// <param name="runContext">The VSTest platform run context.</param>
        /// <param name="frameworkHandle">The VSTest platform framework handle.</param>
        /// <param name="testRunConfiguration">The FlUnit test run configuration to use.</param>
        private void RunTests(
            IEnumerable<TestCase> testCases,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle,
            TestRunConfiguration testRunConfiguration)
        {
            // TODO-ROBUSTNESS: Probably should make sure its not already being run at some point? Throw an invalidoperationexception if so.
            cancellationTokenSource = new CancellationTokenSource();

            var testRun = new TestRun(
                testContainers: testCases.Select(testCase => new TestContainer(testCase, runContext, frameworkHandle)),
                testRunConfiguration);

            testRun.Execute(cancellationTokenSource.Token);
        }
    }
}
