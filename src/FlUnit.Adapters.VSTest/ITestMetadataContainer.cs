using System;
namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface for containers of test metadata. Test adapters must implement this in their representations
    /// of tests to run, that they pass to the <see cref="TestRun{TTestDescriptor}"/> for execution.
    /// </summary>
    internal interface ITestMetadataContainer
    {
        /// <summary>
        /// Gets the FlUnit metadata for this test (which FlUnit execution logic - notably the <see cref="TestRun{TTestDescriptor}"/> class - can use to run it).
        /// </summary>
        TestMetadata TestMetadata { get; }
    }
}
