using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Linq;
using System.Reflection;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// The VSTest adapter's implementation of <see cref="ITestContainer"/>.
    /// Intended for consumption by FlUnit's core execution logic (i.e. the <see cref="TestRun"/> class).
    /// </summary>
    internal class TestContainer : ITestContainer
    {
        private readonly IRunContext runContext;
        private readonly IFrameworkHandle frameworkHandle;
        private readonly TestCase testCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestContainer"/> class.
        /// </summary>
        /// <param name="testCase">The VSTest platform information for the test</param>
        /// <param name="runContext">The VSTest <see cref="IRunContext"/> that the test is being executed in.</param>
        /// <param name="frameworkHandle">The VSTest <see cref="IFrameworkHandle"/> that the test should use for callbacks.</param>
        public TestContainer(TestCase testCase, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            ////DumpTestCase(testCase, frameworkHandle);

            var propertyDetails = ((string)testCase.GetPropertyValue(TestProperties.FlUnitTestProp)).Split(':');
            var assembly = Assembly.Load(propertyDetails[0]); // Might already be loaded - not sure of best practices here. Also, expensive call(s) in a ctor is not ideal (though this class is internal..). Fine for now..
            var type = assembly.GetType(propertyDetails[1]);
            var propertyInfo = type.GetProperty(propertyDetails[2]);

            this.testCase = testCase;
            TestMetadata = new TestMetadata(propertyInfo, testCase.Traits.Select(t => new Trait(t.Name, t.Value)));
            this.runContext = runContext;
            this.frameworkHandle = frameworkHandle;
        }

        /// <summary>
        /// Gets the FlUnit metadata for the test.
        /// </summary>
        public TestMetadata TestMetadata { get; }

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
            switch (flUnitOutome)
            {
                case TestOutcome.Passed:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Passed;
                case TestOutcome.Failed:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Failed;
                case TestOutcome.ArrangementFailed:
                    return Microsoft.VisualStudio.TestPlatform.ObjectModel.TestOutcome.Skipped;
                default:
                    throw new ArgumentException();
            };
        }

        ////private static void DumpTestCase(TestCase testCase, IFrameworkHandle frameworkHandle)
        ////{
        ////    foreach (var property in testCase.Properties)
        ////    {
        ////        frameworkHandle.SendMessage(TestMessageLevel.Informational, $"PROP: {property.Id} - {property.Label} - {property.ValueType}");
        ////    }
        ////}
    }
}
