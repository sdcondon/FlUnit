using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
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
            var testRunConfiguration = TestRunConfiguration.ReadFromXml(runContext.RunSettings?.SettingsXml, Constants.FlUnitConfigurationXmlElement);

            RunTests(
                TestDiscoverer.MakeTestCases(sources, runContext, null, testRunConfiguration),
                runContext,
                frameworkHandle,
                testRunConfiguration);
        }

        /// <inheritdoc />
        public void RunTests(
            IEnumerable<TestCase> tests,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            var testRunConfiguration = TestRunConfiguration.ReadFromXml(runContext.RunSettings?.SettingsXml, Constants.FlUnitConfigurationXmlElement);

            RunTests(
                tests,
                runContext,
                frameworkHandle,
                testRunConfiguration);
        }

        /// <inheritdoc />
        public void Cancel()
        {
            // NB: we don't throw if not in progress - idempotence feels like a useful property for cancellation.
            // The same reasoning doesn't apply to RunTests because the invocation details (test cases etc.) could be different for each invocation..
            // I'd feel comfortable doing it if we included a check that it was the "same" run and only threw if it was different (otherwise just return),
            // but that's way too much effort for now and probably ever..
            cancellationTokenSource?.Cancel();
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
            // NB: no thread (i.e. re-entry) safety here. VSTest adapters for other frameworks don't seem to bother,
            // so presumably the platform is well-behaved in this regard, making it un-needed.
            if (cancellationTokenSource != null)
            {
                throw new InvalidOperationException("Test run already in progress");
            }

            try
            {
                cancellationTokenSource = new CancellationTokenSource();

                var testRun = new TestRun(
                    testContainers: testCases.Select(testCase => new TestContainer(testCase, runContext, frameworkHandle)),
                    testRunConfiguration);

                testRun.Execute(cancellationTokenSource.Token);
            }
            finally
            {
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = null;
            }
        }
    }
}
