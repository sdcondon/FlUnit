using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FlUnit.Adapters
{
    /// <summary>
    /// FlUnit's core test execution logic. Instances of this class encapsulate a FlUnit test run.
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
            if (testRunConfiguration.Parallelise)
            {
                Parallel.ForEach(
                    testContainers,
                    new ParallelOptions()
                    {
                        CancellationToken = cancellationToken
                    },
                    tc => RunTest(tc, testRunConfiguration.TestConfiguration));
            }
            else
            {
                foreach (var testContainer in testContainers)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    RunTest(testContainer, testRunConfiguration.TestConfiguration);
                }
            }
        }

        private void RunTest(ITestContainer testContainer, TestConfiguration testConfiguration)
        {
            var test = (Test)testContainer.TestMetadata.TestProperty.GetValue(null);

            if (test.HasConfigurationOverrides)
            {
                testConfiguration = testConfiguration.Clone();
                test.ApplyConfigurationOverrides(testConfiguration);
            }

            testContainer.RecordStart();

            var testArrangementPassed = TryArrangeTestInstance(test, testContainer, testConfiguration);
            var allAssertionsPassed = testArrangementPassed;
            if (testArrangementPassed)
            {
                foreach (var testCase in test.Cases)
                {
                    var actionStart = DateTimeOffset.Now;
                    testCase.Act();
                    var actionEnd = DateTimeOffset.Now;

                    foreach (var assertion in testCase.Assertions)
                    {
                        allAssertionsPassed &= CheckTestAssertion(test, testCase, actionStart, actionEnd, assertion, testConfiguration, testContainer);
                    }
                }
            }

            TestOutcome testOutcome;
            if (!testArrangementPassed)
            {
                testOutcome = testConfiguration.FailedArrangementOutcomeIsSkipped ? TestOutcome.Skipped : TestOutcome.Failed;
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

        private static bool TryArrangeTestInstance(Test test, ITestContainer testContainer, ITestConfiguration testConfiguration)
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
                // For v1, perhaps just trim the FlUnit stuff from the bottom of the stack trace.
                testContainer.RecordResult(
                    startTime: arrangementStartTime,
                    endTime: DateTimeOffset.Now,
                    displayName: null,
                    outcome: testConfiguration.FailedArrangementOutcomeIsSkipped ? TestOutcome.Skipped : TestOutcome.Failed,
                    errorMessage: string.Format(Messages.ArrangementFailureMessageFormat, e.Message),
                    errorStackTrace: e.StackTrace);

                return false;
            }
        }

        private static bool CheckTestAssertion(Test test, ITestCase testCase, DateTimeOffset actionStart, DateTimeOffset actionEnd, ITestAssertion assertion, ITestConfiguration testConfiguration, ITestContainer testContainer)
        {
            // NB: We use the start and end time for the test action as the start and end time for each assertion result.
            // The assumption being that assertions themselves will generally be (fast and) less interesting.
            // This is something to consider configurability for at some point.
            var startTime = actionStart;
            var endTime = actionEnd;
            string displayName = null;
            var outcome = TestOutcome.Skipped;
            string errorMessage = null;
            string errorStackTrace = null;

            try
            {
                displayName = testConfiguration.ResultNamingStrategy.GetResultName(test, testCase, assertion);

                assertion.Invoke();
                outcome = TestOutcome.Passed;
                return true;
            }
            catch (Exception e)
            {
                // TODO: would need to do a bit more work for good failure messages, esp the stack trace..
                // For v1, perhaps just trim the FlUnit stuff from the bottom of the stack trace.
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
