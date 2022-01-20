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
        /// <param name="propertyInfo"></param>
        /// <param name="traitProviders"></param>
        public TestMetadata(PropertyInfo propertyInfo, IEnumerable<ITraitProvider> traitProviders)
        {
            TestProperty = propertyInfo;
            TraitProviders = traitProviders;
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> the <see cref="Test"/>-valued property that represents the test.
        /// </summary>
        public PropertyInfo TestProperty { get; }

        /// <summary>
        /// Gets an enumerable of the trait providers that are applicable to this test.
        /// </summary>
        public IEnumerable<ITraitProvider> TraitProviders { get; }
    }
}
