using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
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
    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private bool isCancelled = false;

        public void RunTests(
            IEnumerable<string> sources,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            RunTests(TestDiscoverer.MakeTestCases(sources, runContext, null), runContext, frameworkHandle);
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

        private static void RunTestCase(TestCase testCase, IFrameworkHandle frameworkHandle)
        {
            var propertyDetails = ((string)testCase.GetPropertyValue(TestProperties.FlUnitTestProp)).Split(':');
            var assembly = Assembly.Load(propertyDetails[0]);
            var type = assembly.GetType(propertyDetails[1]);
            var propertyInfo = type.GetProperty(propertyDetails[2]);
            var test = (Test)propertyInfo.GetValue(null);

            frameworkHandle.RecordStart(testCase);

            var testArrangementPassed = TryArrangeTestInstance(testCase, test, frameworkHandle);
            var allAssertionsPassed = testArrangementPassed;
            if (testArrangementPassed)
            {
                foreach (var flCase in test.Cases)
                {
                    var flCaseStart = DateTimeOffset.Now;
                    flCase.Act();
                    var flCaseEnd = DateTimeOffset.Now;

                    foreach (var assertion in flCase.Assertions)
                    {
                        allAssertionsPassed &= CheckTestAssertion(testCase, test, flCase, flCaseStart, flCaseEnd, assertion, frameworkHandle);
                    }
                }
            }

            TestOutcome testOutcome;
            if (!testArrangementPassed)
            {
                testOutcome = TestOutcome.Skipped;
            }
            else if (!allAssertionsPassed)
            {
                testOutcome = TestOutcome.Failed;
            }
            else
            {
                testOutcome = TestOutcome.Passed;
            }

            frameworkHandle.RecordEnd(testCase, testOutcome);
        }

        private static bool TryArrangeTestInstance(TestCase testCase, Test test, IFrameworkHandle frameworkHandle)
        {
            var arrangementStartTime = DateTimeOffset.Now;

            try
            {
                test.Arrange();
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                frameworkHandle.RecordResult(new TestResult(testCase)
                {
                    StartTime = arrangementStartTime,
                    Outcome = TestOutcome.Skipped,
                    ErrorMessage = $"Test arrangement failed: {e.Message}", // TODO: localisation needed if this ever takes off
                    ErrorStackTrace = e.StackTrace,
                    EndTime = DateTimeOffset.Now,
                });

                return false;
            }
        }

        private static bool CheckTestAssertion(TestCase testCase, Test flTest, ITestCase flCase, DateTimeOffset flCaseStart, DateTimeOffset flCaseEnd, ITestAssertion testAssertion, IFrameworkHandle frameworkHandle)
        {
            // NB: We use the start and end time for the test action as the start and end time for each assertion result.
            // The assumption being that assertions themselves will generally be (fast and) less interesting.
            var result = new TestResult(testCase)
            {
                StartTime = flCaseStart,
                EndTime = flCaseEnd,
            };

            // Use different descriptions depending on multiplicity of csses and assertions.
            // This makes results in Visual Studio itself look good - but the actual results miss out on some info (so not as good for TRX files).
            // Perhaps room for some configuration of naming strategy at some point.
            if (flTest.Cases.Count > 1 && flCase.Assertions.Count > 1)
            {
                result.DisplayName = string.IsNullOrEmpty(flCase.Description) ? testAssertion.Description : $"{testAssertion.Description} for test case {flCase.Description}";
            }
            else if (flTest.Cases.Count > 1)
            {
                result.DisplayName = flCase.Description;
            }
            else if (flCase.Assertions.Count > 1)
            {
                result.DisplayName = testAssertion.Description;
            }

            try
            {
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
                frameworkHandle.RecordResult(result);
            }
        }
    }
}
