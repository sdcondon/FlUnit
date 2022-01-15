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
    /// <remarks>
    /// References:
    /// <list type="bullet">
    /// <item>https://github.com/microsoft/vstest</item>
    /// <item>https://github.com/microsoft/vstest/tree/master/src/Microsoft.TestPlatform.ObjectModel</item>
    /// <item>https://github.com/microsoft/testfx/tree/master/src/Adapter (MSTest adapter for VSTest)</item>
    /// </list>
    /// </remarks>
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
            var runSettings = RunSettingsReader.ReadXml(discoveryContext.RunSettings.SettingsXml);
            MakeTestCases(sources, discoveryContext, logger, runSettings).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        internal static List<TestCase> MakeTestCases(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            RunSettings runSettings)
        {
            return sources.SelectMany(s => MakeTestCases(s, discoveryContext, logger, runSettings)).ToList();
        }

        private static List<TestCase> MakeTestCases(
            string source,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            RunSettings runSettings)
        {
            var assembly = Assembly.LoadFile(source);

            logger?.SendMessage(TestMessageLevel.Informational, $"Test discovery started for {assembly.FullName}");

            var testProps = FindTestProps(assembly, runSettings);

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

                foreach (var testProp in testProps)
                {
                    var navigationData = diaSession?.GetNavigationData(testProp.propertyInfo.DeclaringType.FullName, testProp.propertyInfo.GetGetMethod().Name);

                    var testCase = new TestCase($"{testProp.propertyInfo.DeclaringType.FullName}.{testProp.propertyInfo.Name}", Constants.ExecutorUri, source)
                    {
                        CodeFilePath = navigationData?.FileName,
                        LineNumber = navigationData?.MinLineNumber ?? 0,
                    };

                    testCase.Traits.AddRange(testProp.traitProviders.Select(t => new Trait(t.Trait.Name, t.Trait.Value)));

                    // Don't need it yet, but I suspect that serializing up references to the traits is ultimately going to be useful, too?
                    testCase.SetPropertyValue(
                        TestProperties.FlUnitTestProp,
                        $"{testProp.propertyInfo.DeclaringType.Assembly.GetName().Name}:{testProp.propertyInfo.DeclaringType.FullName}:{testProp.propertyInfo.Name}"); // Probably better to use JSON or similar..

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

        /// <summary>
        /// Finds all of the properties that represent tests in a given loaded assembly - along with the trait providers that should be applied to each.
        /// </summary>
        /// <param name="assembly">The assembly to look in.</param>
        /// <param name="runSettings">Unused for the moment. Sue me.</param>
        /// <returns>An enumerable of property infos, t</returns>
        /// <remarks>
        /// NB: No dependency on VSTest platform in this method. This is the very early stages of pulling common logic out of the VSTest adapter.
        /// May ultimately need to pass (some kind of abstraction for) the test platform/context as an argument (for e.g. logging). Don't need it yet, though.
        /// <para/>
        /// Ultimately should create some kind of FlUnitTestInfo type rather than using a value tuple. Think about making it serializable so that it can
        /// be used instead of the colon-delimited string currently used. Do this when extracting?
        /// </remarks>
        private static IEnumerable<(PropertyInfo propertyInfo, IEnumerable<ITraitProvider> traitProviders)> FindTestProps(Assembly assembly, RunSettings runSettings)
        {
            (T member, IEnumerable<ITraitProvider> traitProviders) ConcatTraitProviders<T>(T memberInfo, IEnumerable<ITraitProvider> traitProviders)
                where T : MemberInfo
            {
                return (memberInfo, traitProviders: traitProviders.Concat(memberInfo.GetCustomAttributes().OfType<ITraitProvider>()));
            }

            bool IsTestProperty(PropertyInfo p)
            {
                return p.CanRead
                    && p.GetMethod.IsPublic
                    && p.GetMethod.IsStatic
                    && typeof(Test).IsAssignableFrom(p.PropertyType);
            }

            var assemblyTraitProviders = assembly.GetCustomAttributes().OfType<ITraitProvider>();

            return assembly
                .ExportedTypes.Select(t => ConcatTraitProviders(t, assemblyTraitProviders))
                .SelectMany(t => t.member.GetProperties().Where(IsTestProperty).Select(p => ConcatTraitProviders(p, t.traitProviders)));
        }
    }
}
