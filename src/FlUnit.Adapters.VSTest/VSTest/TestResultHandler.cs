using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// The VSTest adapter's implementation of <see cref="ITestResultHandler"/>.
    /// Intended for consumption by FlUnit's core execution logic to pass individual test results back to the VSTest platform.
    /// </summary>
    internal class TestResultHandler : ITestResultHandler
    {
        private readonly IRunContext runContext;
        private readonly IFrameworkHandle frameworkHandle;
        private readonly TestCase testCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultHandler"/> instance.
        /// </summary>
        /// <param name="runContext"></param>
        /// <param name="frameworkHandle"></param>
        /// <param name="testCase"></param>
        public TestResultHandler(IRunContext runContext, IFrameworkHandle frameworkHandle, TestCase testCase)
        {
            this.runContext = runContext;
            this.frameworkHandle = frameworkHandle;
            this.testCase = testCase;
        }

        /// <inheritdoc/>
        public void RecordStart()
        {
            frameworkHandle.RecordStart(testCase);
        }

        /// <inheritdoc/>
        public void RecordResult(
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            string displayName,
            TestOutcome outcome,
            string errorMessage,
            string errorStackTrace)
        {
            frameworkHandle.RecordResult(new TestResult(testCase)
            {
                StartTime = startTime,
                EndTime = endTime,
                DisplayName = displayName,
                Outcome = MapOutcome(outcome),
                ErrorMessage = errorMessage,
                ErrorStackTrace = errorStackTrace,
            });
        }

        /// <inheritdoc/>
        public void RecordEnd(TestOutcome outcome)
        {
            frameworkHandle.RecordEnd(testCase, MapOutcome(outcome));
        }

        private static Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome MapOutcome(TestOutcome flUnitOutome)
        {
            switch(flUnitOutome)
            {
                case TestOutcome.Passed:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Passed;
                case TestOutcome.Failed:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Failed;
                case TestOutcome.Skipped:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Skipped;
                default:
                    throw new ArgumentException();
            };
        }
    }
}
