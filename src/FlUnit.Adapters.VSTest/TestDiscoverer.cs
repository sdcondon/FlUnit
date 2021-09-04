using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using System;

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
            MakeTestCases(sources, discoveryContext, logger).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        internal static List<TestCase> MakeTestCases(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger)
        {
            return sources.SelectMany(s => MakeTestCases(s, discoveryContext, logger)).ToList();
        }

        private static List<TestCase> MakeTestCases(string source, IDiscoveryContext discoveryContext, IMessageLogger logger)
        {
            var assembly = Assembly.LoadFile(source);
            logger?.SendMessage(
                TestMessageLevel.Informational,
                $"Test discovery started for {assembly.FullName}");

            (T member, IEnumerable<ITraitProvider> traitProviders) RollUpTraitProviders<T>(T memberInfo, IEnumerable<ITraitProvider> traitProviders)
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

            var testProps = assembly
                .ExportedTypes.Select(t => RollUpTraitProviders(t, assemblyTraitProviders))
                .SelectMany(t => t.member.GetProperties().Where(IsTestProperty).Select(p => RollUpTraitProviders(p, t.traitProviders)));

            var testCases = new List<TestCase>();
            using (var diaSession = new DiaSession(source))
            {
                foreach (var p in testProps)
                {
                    var testCase = MakeTestCase(source, p.member, p.traitProviders, diaSession);
                    testCases.Add(testCase);
                    logger?.SendMessage(
                        TestMessageLevel.Informational,
                        $"Found test case [{assembly.GetName().Name}]{testCase.FullyQualifiedName}. Traits: {string.Join(", ", testCase.Traits.Select(t => $"{t.Name}={t.Value}"))}");
                }
            }

            return testCases;
        }

        private static TestCase MakeTestCase(string source, PropertyInfo p, IEnumerable<ITraitProvider> traitProviders, DiaSession diaSession)
        {
            var navigationData = diaSession.GetNavigationData(p.DeclaringType.FullName, p.GetGetMethod().Name);

            var testCase = new TestCase($"{p.DeclaringType.FullName}.{p.Name}", Constants.ExecutorUri, source)
            {
                CodeFilePath = navigationData.FileName,
                LineNumber = navigationData.MinLineNumber,
            };

            testCase.Traits.AddRange(traitProviders.Select(t => new Trait(t.Trait.Name, t.Trait.Value)));

            testCase.SetPropertyValue(
                TestProperties.FlUnitTestProp,
                $"{p.DeclaringType.Assembly.GetName().Name}:{p.DeclaringType.FullName}:{p.Name}"); // Perhaps better to use JSON or similar..

            return testCase;
        }
    }
}
