using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    // References:
    // https://github.com/microsoft/vstest
    // https://github.com/microsoft/vstest/tree/master/src/Microsoft.TestPlatform.ObjectModel
    // https://github.com/xunit/visualstudio.xunit/tree/master/src/xunit.runner.visualstudio

    [FileExtension(".exe")]
    [FileExtension(".dll")]
    [DefaultExecutorUri(ExecutorUriString)]
    [ExtensionUri(ExecutorUriString)]
    public class TestRunner : ITestDiscoverer, ITestExecutor
    {
        public const string ExecutorUriString = "executor://FlUnitTestRunner";
        public static readonly Uri ExecutorUri = new Uri(ExecutorUriString);

        // NB: for some unfathomable reason, property ID has to be Pascal-cased, with the framework raising an unguessable error message for camel-casing.
        // But not until after the property has been registered. A violated assumption during the serialization process, maybe?
        private static readonly TestProperty FlUnitTestProp = TestProperty.Register("FlUnitTestCase", "flUnit Test Case", typeof(string), typeof(TestRunner));

        private bool isCancelled = false;

        public void DiscoverTests(
            IEnumerable<string> sources,
            IDiscoveryContext discoveryContext,
            IMessageLogger logger,
            ITestCaseDiscoverySink discoverySink)
        {
            MakeTestCases(sources, discoveryContext, logger).ForEach(tc => discoverySink.SendTestCase(tc));
        }

        public void RunTests(
            IEnumerable<string> sources,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            RunTests(MakeTestCases(sources, runContext, null), runContext, frameworkHandle);
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

                RunTestCase(test, frameworkHandle);
            }
        }

        public void Cancel()
        {
            isCancelled = true;
        }

        private static List<TestCase> MakeTestCases(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger)
        {
            return sources.SelectMany(s => MakeTestCases(s, discoveryContext, logger)).ToList();
        }

        private static List<TestCase> MakeTestCases(string source, IDiscoveryContext discoveryContext, IMessageLogger logger)
        {
            var assembly = Assembly.LoadFile(source);

            var testProps = assembly.ExportedTypes
                .SelectMany(c => c.GetProperties())
                .Where(p => p.PropertyType == typeof(ITest));
            var testCases = new List<TestCase>();
            foreach (var p in testProps)
            {
                var testCase = MakeTestCase(source, p);
                testCases.Add(testCase);
                logger?.SendMessage(
                    TestMessageLevel.Informational,
                    $"Found test case [{assembly.GetName().Name}]{testCase.FullyQualifiedName}");
            }

            return testCases;
        }

        private static TestCase MakeTestCase(string source, PropertyInfo p)
        {
            var testCase = new TestCase($"{p.DeclaringType.FullName}.{p.Name}", ExecutorUri, source)
            {
                //CodeFilePath = ..,
                //LineNumber = ..,
            };
            testCase.SetPropertyValue(
                FlUnitTestProp,
                $"{p.DeclaringType.Assembly.GetName()}:{p.DeclaringType.FullName}:{p.Name}"); // Perhaps better to use JSON or similar..
            return testCase;
        }

        private static void RunTestCase(TestCase testCase, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.RecordStart(testCase);
            var result = new TestResult(testCase);

            try
            {
                var propertyDetails = ((string)testCase.GetPropertyValue(FlUnitTestProp)).Split(':');
                var assembly = Assembly.Load(propertyDetails[0]);
                var type = assembly.GetType(propertyDetails[1]);
                var propertyInfo = type.GetProperty(propertyDetails[2]);
                var test = (ITest)propertyInfo.GetValue(null);

                result.StartTime = DateTimeOffset.Now;
                test.Run();

                foreach (var assertion in test.Assertions)
                {
                    assertion.Item1();
                }

                result.Outcome = TestOutcome.Passed;
            }
            catch (Exception e)
            {
                result.Outcome = TestOutcome.Failed;
                result.ErrorMessage = e.Message;
                result.ErrorStackTrace = e.StackTrace;
            }
            finally
            {
                result.EndTime = DateTimeOffset.Now;
            }

            frameworkHandle.RecordEnd(testCase, result.Outcome);
            frameworkHandle.RecordResult(result);
        }
    }
}
