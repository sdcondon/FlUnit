using System;
namespace FlUnit.Adapters
{
    /// <summary>
    /// Interface for containers of test metadata. Test adapters should implement this within their representations of tests to run - 
    /// the generic type argument of <see cref="TestRun{TTestDescriptor}"/> must implement this interface.
    /// </summary>
    internal interface ITestMetadataContainer
    {
        /// <summary>
        /// Gets the FlUnit metadata for this test (which FlUnit execution logic - notably the <see cref="TestRun{TTestDescriptor}"/> class - can use to run it).
        /// </summary>
        TestMetadata TestMetadata { get; }
    }
}
