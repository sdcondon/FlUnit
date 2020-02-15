using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    // TODO: This class probably needs splitting up - want to take a closer look at how MSTest V2 runner is structured (it looks quite a bit more organised than XUnit one..)
    //
    // References:
    // https://github.com/microsoft/vstest
    // https://github.com/microsoft/vstest/tree/master/src/Microsoft.TestPlatform.ObjectModel
    // https://github.com/microsoft/testfx (MSTest)
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
            logger?.SendMessage(
                TestMessageLevel.Informational,
                $"Test discovery started for {assembly.FullName}");

            var testProps = assembly.ExportedTypes
                .SelectMany(c => c.GetProperties())
                .Where(p => p.PropertyType == typeof(ITest));

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

            var testCase = new TestCase($"{p.DeclaringType.FullName}.{p.Name}", ExecutorUri, source)
            {
                CodeFilePath = navigationData.FileName,
                LineNumber = navigationData.MinLineNumber
            };
            testCase.SetPropertyValue(
                FlUnitTestProp,
                $"{p.DeclaringType.Assembly.GetName().Name}:{p.DeclaringType.FullName}:{p.Name}"); // Perhaps better to use JSON or similar..
            return testCase;
        }

        private static void RunTestCase(TestCase testCase, IFrameworkHandle frameworkHandle)
        {
            frameworkHandle.RecordStart(testCase);

            //// TODO: the same number of results should always be recorded - just with a "None" outcome if it didn't get that far..

            if (!TryMakeTestInstance(testCase, frameworkHandle, out var test))
            {
                frameworkHandle.RecordEnd(testCase, TestOutcome.None);
                return;
            }

            if(!RunTestInstance(testCase, test, frameworkHandle))
            {
                frameworkHandle.RecordEnd(testCase, TestOutcome.Failed);
                return;
            }

            var passed = true;
            foreach (var assertion in test.Assertions)
            {
                passed &= CheckTestAssertion(testCase, assertion, frameworkHandle);
            }
            frameworkHandle.RecordEnd(testCase, passed ? TestOutcome.Passed : TestOutcome.Failed);
        }

        private static bool TryMakeTestInstance(TestCase testCase, IFrameworkHandle frameworkHandle, out ITest test)
        {
            var result = new TestResult(testCase)
            {
                DisplayName = "[Test Arrangement]"
            };

            try
            {
                var propertyDetails = ((string)testCase.GetPropertyValue(FlUnitTestProp)).Split(':');
                var assembly = Assembly.Load(propertyDetails[0]);
                var type = assembly.GetType(propertyDetails[1]);
                var propertyInfo = type.GetProperty(propertyDetails[2]);

                result.StartTime = DateTimeOffset.Now;
                test = (ITest)propertyInfo.GetValue(null);
                result.Outcome = TestOutcome.Passed;
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                result.Outcome = TestOutcome.Failed;
                result.ErrorMessage = e.Message;
                result.ErrorStackTrace = e.StackTrace;

                test = null;
                return false;
            }
            finally
            {
                result.EndTime = DateTimeOffset.Now;
                frameworkHandle.RecordResult(result);
            }
        }

        private static bool RunTestInstance(TestCase testCase, ITest test, IFrameworkHandle frameworkHandle)
        {
            var result = new TestResult(testCase)
            {
                DisplayName = "[Test Action]"
            };

            try
            {
                result.StartTime = DateTimeOffset.Now;
                test.Act();

                result.Outcome = TestOutcome.Passed;
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                result.Outcome = TestOutcome.Failed;
                result.ErrorMessage = e.Message;
                result.ErrorStackTrace = e.StackTrace;
                return false;
            }
            finally
            {
                result.EndTime = DateTimeOffset.Now;
                frameworkHandle.RecordResult(result);
            }
        }

        private static bool CheckTestAssertion(TestCase testCase, TestAssertion testAssertion, IFrameworkHandle frameworkHandle)
        {
            var result = new TestResult(testCase)
            {
                DisplayName = testAssertion.Description
            };

            try
            {
                result.StartTime = DateTimeOffset.Now;
                testAssertion.Invoke();

                result.Outcome = TestOutcome.Passed;
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                result.Outcome = TestOutcome.Failed;
                result.ErrorMessage = e.Message;
                result.ErrorStackTrace = e.StackTrace;
                return false;
            }
            finally
            {
                result.EndTime = DateTimeOffset.Now;
                frameworkHandle.RecordResult(result);
            }
        }
    }
}
