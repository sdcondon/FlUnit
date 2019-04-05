namespace FlUnit.Adapters.VSTest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using System.Collections.Generic;

    [FileExtension(".xml")]
    [DefaultExecutorUri("executor://FlUnitTestExecutor")]
    class TestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> containers, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            // Logic to get the tests from the containers passed in.
            var testsFound = new TestCase[0];

            //Notify the test platform of the list of test cases found.
            foreach (TestCase test in testsFound)
            {
                discoverySink.SendTestCase(test);
            }
        }
    }

    [ExtensionUri("executor://FlUnitTestExecutor")]
    class TestExecutor : ITestExecutor
    {
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            // Logic to run xml based test cases and report back results.
        }

        public void RunTests(IEnumerable<string> containers, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            // Logic to discover and run xml based tests and report back results.
        }

        public void Cancel()
        {
            // Logic to cancel the current test run.
        }
    }
}
