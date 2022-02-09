using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using System.IO;

    /// <summary>
    /// FlUnit's implementation of <see cref="ITestDiscoverer"/> - takes responsibility for discovering tests in a given assembly or assemblies.
    /// </summary>
    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [DefaultExecutorUri(Constants.ExecutorUriString)]
    public class TestDiscoverer : ITestDiscoverer
    {
        /// <inheritdoc />
        public void DiscoverTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            var testRunConfiguration = TestRunConfiguration.ReadFromXml(discoveryContext.RunSettings?.SettingsXml, Constants.FlUnitConfigurationXmlElement);
            MakeTestCases(sources, discoveryContext, logger, testRunConfiguration).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        internal static List<TestCase> MakeTestCases(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            TestRunConfiguration testRunConfiguration)
        {
            return sources.SelectMany(s => MakeTestCases(s, discoveryContext, logger, testRunConfiguration)).ToList();
        }

        private static List<TestCase> MakeTestCases(
            string source,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            TestRunConfiguration testRunConfiguration)
        {
            var assembly = Assembly.LoadFile(source); // TODO: check exactly how other adapters go about this

            logger?.SendMessage(TestMessageLevel.Informational, $"Test discovery started for {assembly.FullName}");

            var testMetadata = TestDiscovery.FindTests(assembly, testRunConfiguration);

            var testCases = new List<TestCase>();
            DiaSession diaSession = null;
            try
            {
                try
                {
                    diaSession = new DiaSession(source);
                }
                catch (FileNotFoundException)
                {
                }

                foreach (var testMetadatum in testMetadata)
                {
                    var navigationData = diaSession?.GetNavigationData(testMetadatum.TestProperty.DeclaringType.FullName, testMetadatum.TestProperty.GetGetMethod().Name);

                    var testCase = new TestCase($"{testMetadatum.TestProperty.DeclaringType.FullName}.{testMetadatum.TestProperty.Name}", Constants.ExecutorUri, source)
                    {
                        CodeFilePath = navigationData?.FileName,
                        LineNumber = navigationData?.MinLineNumber ?? 0,
                    };

                    testCase.Traits.AddRange(testMetadatum.Traits.Select(t => new Trait(t.Name, t.Value)));

                    // Probably better to use JSON or similar..
                    // ..and in general need to pay more attention to how the serialization between discovery and execution works..
                    // ..e.g. does the serialised version stick around? Do I need to worry about versioning test cases and executor version?
                    testCase.SetPropertyValue(
                        TestProperties.FlUnitTestProp,
                        $"{testMetadatum.TestProperty.DeclaringType.Assembly.GetName().Name}:{testMetadatum.TestProperty.DeclaringType.FullName}:{testMetadatum.TestProperty.Name}");

                    testCases.Add(testCase);

                    // TODO: Neater message when there are no traits (or simply don't include them - was only a quick and dirty test)
                    logger?.SendMessage(
                        TestMessageLevel.Informational,
                        $"Found test case [{assembly.GetName().Name}]{testCase.FullyQualifiedName}. Traits: {string.Join(", ", testCase.Traits.Select(t => $"{t.Name}={t.Value}"))}");
                }
            }
            finally
            {
                diaSession?.Dispose();
            }

            return testCases;
        }
    }
}
