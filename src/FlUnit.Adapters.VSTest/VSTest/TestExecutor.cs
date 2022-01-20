using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using System.Linq;
    using System.Threading;

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
        /// Private method for running tests once we have determined which tests we want to run and the run settings to use.
        /// </summary>
        /// <param name="tests">The tests to run.</param>
        /// <param name="runContext">The VSTest platform run context.</param>
        /// <param name="frameworkHandle">The VSTest platform framework handle.</param>
        /// <param name="runSettings">The run settings to use.</param>
        private void RunTests(
            IEnumerable<TestCase> tests,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle,
            Adapters.TestRunConfiguration runSettings)
        {
            // TODO-ROBUSTNESS: Probably should make sure its not already being run at some point? Throw an invalidoperationexception if so.
            cancellationTokenSource = new CancellationTokenSource();

            var testContainers = tests.Select(t =>
            {
                ////DumpTestCase(t, frameworkHandle);

                var propertyDetails = ((string)t.GetPropertyValue(TestProperties.FlUnitTestProp)).Split(':');
                var assembly = Assembly.Load(propertyDetails[0]);
                var type = assembly.GetType(propertyDetails[1]);
                var propertyInfo = type.GetProperty(propertyDetails[2]);

                // TODO-FUNCTIONALITY/MAINTAINABILITY: null traitproviders. Will need to populate these to have traits that affect execution?
                // Or tidy this up so that traits are only a test discovery thing. Messy at the mo.
                // If we want them just re-find them, I guess? Serializing would be needlessly complex. But re-finding them is potentially slow of we aren't clever about it..
                // ANSWER: Traits exist as a VS TestCase prop - should we change metadata so that it includes traits, not traitproviders? 
                return new TestContainer(t, new TestMetadata(propertyInfo, null), runContext, frameworkHandle);
            });

            var testRun = new TestRun(testContainers, runSettings);

            testRun.Execute(cancellationTokenSource.Token);
        }

        ////private void DumpTestCase(TestCase testCase, IFrameworkHandle frameworkHandle)
        ////{
        ////    foreach(var property in testCase.Properties)
        ////    {
        ////        frameworkHandle.SendMessage(TestMessageLevel.Informational, $"PROP: {property.Id} - {property.Label} - {property.ValueType}");
        ////    }
        ////}
    }
}
