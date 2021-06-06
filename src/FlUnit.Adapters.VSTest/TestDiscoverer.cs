using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    // TODO: This class probably needs splitting up - want to take a closer look at how MSTest V2 runner is structured (it looks quite a bit more organised than XUnit one..)
    //
    // References:
    // https://github.com/microsoft/vstest
    // https://github.com/microsoft/vstest/tree/master/src/Microsoft.TestPlatform.ObjectModel
    // https://github.com/microsoft/testfx/tree/master/src/Adapter (MSTest adapter for VSTest)

    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [DefaultExecutorUri(Constants.ExecutorUriString)]
    public class TestDiscoverer : ITestDiscoverer
    {
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

            var testProps = assembly.ExportedTypes
                .SelectMany(c => c.GetProperties(BindingFlags.Public | BindingFlags.Static))
                .Where(p => p.PropertyType == typeof(Test) && p.CanRead);

            var testCases = new List<TestCase>();
            using (var diaSession = new DiaSession(source))
            {
                foreach (var p in testProps)
                {
                    var testCase = MakeTestCase(source, p, diaSession);
                    testCases.Add(testCase);
                    logger?.SendMessage(
                        TestMessageLevel.Informational,
                        $"Found test case [{assembly.GetName().Name}]{testCase.FullyQualifiedName}");
                }
            }

            return testCases;
        }

        private static TestCase MakeTestCase(string source, PropertyInfo p, DiaSession diaSession)
        {
            var navigationData = diaSession.GetNavigationData(p.DeclaringType.FullName, p.GetGetMethod().Name);

            var testCase = new TestCase($"{p.DeclaringType.FullName}.{p.Name}", Constants.ExecutorUri, source)
            {
                CodeFilePath = navigationData.FileName,
                LineNumber = navigationData.MinLineNumber
            };

            testCase.SetPropertyValue(
                TestProperties.FlUnitTestProp,
                $"{p.DeclaringType.Assembly.GetName().Name}:{p.DeclaringType.FullName}:{p.Name}"); // Perhaps better to use JSON or similar..

            return testCase;
        }
    }
}
