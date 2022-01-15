using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;

    /// <summary>
    /// FlUnit's implementation of <see cref="ITestExecutor"/> - takes responsibility for executing tests as discovered by <see cref="TestDiscoverer"/>.
    /// </summary>
    /// <remarks>
    /// References:
    /// <list type="bullet">
    /// <item>https://github.com/microsoft/vstest</item>
    /// <item>https://github.com/microsoft/vstest/tree/master/src/Microsoft.TestPlatform.ObjectModel</item>
    /// <item>https://github.com/microsoft/testfx/tree/master/src/Adapter (MSTest adapter for VSTest)</item>
    /// </list>
    /// </remarks>
    [ExtensionUri(Constants.ExecutorUriString)]
    public class TestExecutor : ITestExecutor
    {
        private bool isCancelled = false; // TODO: should probably be a cancellationtoken instead - deal with this when looking into parallelisation

        /// <inheritdoc />
        public void RunTests(
            IEnumerable<string> sources,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle)
        {
            var runSettings = RunSettingsReader.ReadXml(runContext.RunSettings?.SettingsXml);

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
            var runSettings = RunSettingsReader.ReadXml(runContext.RunSettings?.SettingsXml);

            RunTests(
                tests,
                runContext,
                frameworkHandle,
                runSettings);
        }

        private void RunTests(
            IEnumerable<TestCase> tests,
            IRunContext runContext,
            IFrameworkHandle frameworkHandle,
            RunSettings runSettings)
        {
            isCancelled = false;

            // NB: We only run tests sequentially for the moment.
            // Once parallelisation is supported, the actual test run will be encapsulated away from this class.
            foreach (TestCase test in tests)
            {
                if (isCancelled)
                {
                    break;
                }

                RunTestCase(test, runContext, frameworkHandle, runSettings);
            }
        }

        /// <inheritdoc />
        public void Cancel()
        {
            isCancelled = true;
        }

        private static void RunTestCase(TestCase testCase, IRunContext runContext, IFrameworkHandle frameworkHandle, RunSettings runSettings)
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
                    ErrorMessage = $"Test arrangement failed: {e.Message}", // TODO-LOCALISATION: localisation needed if this ever takes off
                    ErrorStackTrace = e.StackTrace,
                    EndTime = DateTimeOffset.Now,
                });

                return false;
            }
        }

        private static bool CheckTestAssertion(TestCase vsCase, Test flTest, ITestCase flCase, DateTimeOffset flCaseStart, DateTimeOffset flCaseEnd, ITestAssertion flAssertion, IFrameworkHandle vsFrameworkHandle)
        {
            // NB: We use the start and end time for the test action as the start and end time for each assertion result.
            // The assumption being that assertions themselves will generally be (fast and) less interesting.
            // This is something to consider configurability for at some point.
            var vsResult = new TestResult(vsCase)
            {
                StartTime = flCaseStart,
                EndTime = flCaseEnd,
            };

            // Use different descriptions depending on multiplicity of cases and assertions.
            // This makes results in Visual Studio itself look good - but the actual results miss out on some info (so not as good for TRX files).
            // As with duration, there is room for some configuration of naming strategy at some point.
            if (flTest.Cases.Count > 1 && flCase.Assertions.Count > 1)
            {
                vsResult.DisplayName = string.IsNullOrEmpty(flCase.Description) ? flAssertion.Description : $"{flAssertion.Description} for test case {flCase.Description}"; // TODO-LOCALISATION: localisation needed if this ever takes off
            }
            else if (flTest.Cases.Count > 1)
            {
                vsResult.DisplayName = flCase.Description;
            }
            else if (flCase.Assertions.Count > 1)
            {
                vsResult.DisplayName = flAssertion.Description;
            }

            try
            {
                flAssertion.Invoke();
                vsResult.Outcome = TestOutcome.Passed;
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                vsResult.Outcome = TestOutcome.Failed;
                vsResult.ErrorMessage = e.Message;
                vsResult.ErrorStackTrace = e.StackTrace;
                return false;
            }
            finally
            {
                vsFrameworkHandle.RecordResult(vsResult);
            }
        }
    }
}
