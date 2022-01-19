using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// Container for test information - VSTest platform <see cref="Microsoft.VisualStudio.TestPlatform.ObjectModel.TestCase"/> info and FlUnit <see cref="FlUnit.TestMetadata"/>.
    /// Intended for consumption by FlUnit's core execution logic (i.e. <see cref="TestRun{TTestDescriptor}"/>).
    /// </summary>
    internal class TestDescriptor : ITestMetadataContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDescriptor"/> class.
        /// </summary>
        /// <param name="testCase">The VSTest platform information for the test</param>
        /// <param name="flUnitMetadata">The FlUnit metadata for the test</param>
        public TestDescriptor(TestCase testCase, TestMetadata flUnitMetadata)
        {
            TestCase = testCase;
            TestMetadata = flUnitMetadata;
        }

        /// <summary>
        /// Gets the VSTest platform information for the test.
        /// </summary>
        public TestCase TestCase { get; }

        /// <summary>
        /// Gets the FlUnit metadata for the test.
        /// </summary>
        public TestMetadata TestMetadata { get; }
    }
}
