using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace FlUnit.Adapters.VSTest
{
    /// <summary>
    /// Container for registered VSTest <see cref="TestProperty"/>s that FlUnit needs to represent each test.
    /// </summary>
    internal class TestProperties
    {
        /// <summary>
        /// Test property that stores a reference to the (assembly and) static gettable property that represent the test. 
        /// </summary>
        /// <remarks>
        /// NB: for some unfathomable reason, property ID has to be Pascal-cased, with the framework raising an unguessable error message for camel-casing.
        /// But not until after the property has been registered. A violated assumption during the serialization process, maybe?
        /// </remarks>
        internal static readonly TestProperty FlUnitTestProp = TestProperty.Register("FlUnitTestCase", "flUnit Test Case", typeof(string), typeof(TestExecutor));
    }
}
