using System.Collections.Generic;
using System.Reflection;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Containers of test metadata - enough for the FlUnit execution logic to run a test.
    /// </summary>
    internal class TestMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestMetadata"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="PropertyInfo"/> for the <see cref="Test"/>-valued property that represents the test.</param>
        /// <param name="traits">An enumerable of the traits that are applicable to this test.</param>
        public TestMetadata(PropertyInfo propertyInfo, IEnumerable<ITrait> traits)
        {
            TestProperty = propertyInfo;
            Traits = traits;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the <see cref="Test"/>-valued property that represents the test.
        /// </summary>
        public PropertyInfo TestProperty { get; }

        /// <summary>
        /// Gets an enumerable of the traits that are applicable to this test.
        /// </summary>
        public IEnumerable<ITrait> Traits { get; }
    }
}
