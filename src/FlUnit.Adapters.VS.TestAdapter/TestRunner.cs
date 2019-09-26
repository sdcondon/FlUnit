using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit.Adapters.VSTest
{
    // https://github.com/xunit/visualstudio.xunit

    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [DefaultExecutorUri(ExecutorUriString)]
    [ExtensionUri(ExecutorUriString)]
    public class TestRunner : ITestDiscoverer, ITestExecutor
    {
        public const string ExecutorUriString = "executor://FlUnitTestRunner";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        private bool isCancelled = false;

        public void DiscoverTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            FindTestCases(sources, discoveryContext).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        public void RunTests(
            IEnumerable<string> sources,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            RunTests(FindTestCases(sources, runContext), runContext, frameworkHandle);
        }

        public void RunTests(
            IEnumerable<TestCase> tests,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            isCancelled = false;

            foreach (TestCase test in tests)
            {
                if (isCancelled)
                {
                    break;
                }

                frameworkHandle.RecordResult(RunTestCase(test));
            }
        }

        public void Cancel()
        {
            isCancelled = true;
        }

        private List<TestCase> FindTestCases(IEnumerable<string> sources, IDiscoveryContext discoveryContext)
        {
            return sources.Select(s => new TestCase("Foo.Bar.DUMMY TC", ExecutorUri, s)
            {
                CodeFilePath = s,
            }).ToList();
        }

        private TestResult RunTestCase(TestCase testCase)
        {
            return new TestResult(testCase)
            {
                //Outcome = (TestOutcome)test.GetPropertyValue(TestResultProperties.Outcome),
                Outcome = TestOutcome.Passed
            };
        }
    }
}
