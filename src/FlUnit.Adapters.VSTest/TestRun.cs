using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Core test execution logic. Instances of this class encapsulate a FlUnit test run.
    /// </summary>
    internal class TestRun
    {
        private readonly IEnumerable<ITestContainer> testContainers;
        private readonly TestRunConfiguration testRunConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRun"/> class.
        /// </summary>
        /// <param name="testContainers">An enumerable of <see cref="ITestContainer"/> instances, one for each of the tests to be run.</param>
        /// <param name="testRunConfiguration">The configuration that applies to this test run.</param>
        public TestRun(IEnumerable<ITestContainer> testContainers, TestRunConfiguration testRunConfiguration)
        {
            this.testContainers = testContainers;
            this.testRunConfiguration = testRunConfiguration;
        }

        /// <summary>
        /// Executes the test run.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token that, if triggered, should cause the test run to abort.</param>
        public void Execute(CancellationToken cancellationToken)
        {
            // NB: We only run tests sequentially for the moment.
            // Once parallelisation is supported, this would likely invoke Parallel.ForEach when parallelisation is in use. 
            // At that point, this method likely to become async - in case test runners are in a position to take advantage of that
            // (and ultimately to perhaps allow for async tests?)
            foreach (var testContainer in testContainers)
            {
                cancellationToken.ThrowIfCancellationRequested();
                RunTest(testContainer, testRunConfiguration.TestConfiguration);
            }
        }

        private void RunTest(ITestContainer testContainer, TestConfiguration testConfiguration)
        {
            var test = (Test)testContainer.TestMetadata.TestProperty.GetValue(null);

            testContainer.RecordStart();

            var testArrangementPassed = TryArrangeTestInstance(test, testContainer, testConfiguration);
            var allAssertionsPassed = testArrangementPassed;
            if (testArrangementPassed)
            {
                foreach (var testCase in test.Cases)
                {
                    var caseStart = DateTimeOffset.Now;
                    testCase.Act();
                    var caseEnd = DateTimeOffset.Now;

                    foreach (var assertion in testCase.Assertions)
                    {
                        allAssertionsPassed &= CheckTestAssertion(test, testCase, caseStart, caseEnd, assertion, testConfiguration, testContainer);
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

            testContainer.RecordEnd(testOutcome);
        }

        private static bool TryArrangeTestInstance(Test test, ITestContainer testContainer, TestConfiguration testConfiguration)
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
                testContainer.RecordResult(
                    startTime: arrangementStartTime,
                    endTime: DateTimeOffset.Now,
                    displayName: null,
                    outcome: testConfiguration.ArrangementFailureOutcome,
                    errorMessage: $"Test arrangement failed: {e.Message}", // TODO-LOCALISATION: localisation needed if this ever catches on
                    errorStackTrace: e.StackTrace);

                return false;
            }
        }

        private static bool CheckTestAssertion(Test test, ITestCase testCase, DateTimeOffset caseStart, DateTimeOffset caseEnd, ITestAssertion assertion, TestConfiguration testConfiguration, ITestContainer testContainer)
        {
            // NB: We use the start and end time for the test action as the start and end time for each assertion result.
            // The assumption being that assertions themselves will generally be (fast and) less interesting.
            // This is something to consider configurability for at some point.
            var startTime = caseStart;
            var endTime = caseEnd;
            string displayName = null;
            var outcome = TestOutcome.Skipped;
            string errorMessage = null;
            string errorStackTrace = null;

            try
            {
                // Use different descriptions depending on multiplicity of cases and assertions.
                // This makes results in Visual Studio itself look good - but the actual results miss out on some info (so not as good for TRX files).
                // As with duration, there is room for some configuration of naming strategy at some point.
                if (test.Cases.Count > 1 && testCase.Assertions.Count > 1)
                {
                    displayName = string.IsNullOrEmpty(testCase.Description)
                        ? assertion.Description
                        : $"{assertion.Description} for test case {testCase.Description}"; // TODO-LOCALISATION: localisation needed if this ever catches on
                }
                else if (test.Cases.Count > 1)
                {
                    displayName = testCase.Description;
                }
                else if (testCase.Assertions.Count > 1)
                {
                    displayName = assertion.Description;
                }

                assertion.Invoke();
                outcome = TestOutcome.Passed;
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                outcome = TestOutcome.Failed;
                errorMessage = e.Message;
                errorStackTrace = e.StackTrace;
                return false;
            }
            finally
            {
                testContainer.RecordResult(
                    startTime,
                    endTime,
                    displayName,
                    outcome,
                    errorMessage,
                    errorStackTrace);
            }
        }
    }
}
