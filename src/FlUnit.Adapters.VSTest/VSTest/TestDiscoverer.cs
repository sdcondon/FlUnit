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
            var runSettings = TestRunConfigurationReader.ReadXml(discoveryContext.RunSettings.SettingsXml);
            MakeTestCases(sources, discoveryContext, logger, runSettings).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        internal static List<TestCase> MakeTestCases(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            Adapters.TestRunConfiguration runSettings)
        {
            return sources.SelectMany(s => MakeTestCases(s, discoveryContext, logger, runSettings)).ToList();
        }

        private static List<TestCase> MakeTestCases(
            string source,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            Adapters.TestRunConfiguration runSettings)
        {
            var assembly = Assembly.LoadFile(source);

            logger?.SendMessage(TestMessageLevel.Informational, $"Test discovery started for {assembly.FullName}");

            var testMetadata = TestDiscovery.FindTests(assembly, runSettings);

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

                    testCase.Traits.AddRange(testMetadatum.TraitProviders.Select(t => new Trait(t.Trait.Name, t.Trait.Value)));

                    // Probably better to use JSON or similar..
                    // ..and in general need to pay more attention to how the serialization between discovery and execution works..
                    testCase.SetPropertyValue(
                        TestProperties.FlUnitTestProp,
                        $"{testMetadatum.TestProperty.DeclaringType.Assembly.GetName().Name}:{testMetadatum.TestProperty.DeclaringType.FullName}:{testMetadatum.TestProperty.Name}");

                    testCases.Add(testCase);
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
