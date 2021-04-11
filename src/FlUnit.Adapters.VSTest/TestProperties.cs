using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace FlUnit.Adapters.VSTest
{
    internal class TestProperties
    {
        // NB: for some unfathomable reason, property ID has to be Pascal-cased, with the framework raising an unguessable error message for camel-casing.
        // But not until after the property has been registered. A violated assumption during the serialization process, maybe?
        internal static readonly TestProperty FlUnitTestProp = TestProperty.Register("FlUnitTestCase", "flUnit Test Case", typeof(string), typeof(TestExecutor));
    }
}
