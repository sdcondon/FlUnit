using FlUnit.Configuration;

namespace FlUnit.Adapters
{
    /// <summary>
    /// The default strategy for naming test results.
    /// </summary>
    internal class DefaultResultNamingStrategy : IResultNamingStrategy
    {
        /// <inheritdoc />
        public string GetResultName(Test test, ITestCase testCase, ITestAssertion assertion)
        {
            // Use different descriptions depending on multiplicity of cases and assertions.
            // This makes results in Visual Studio itself look good - but the actual results miss out on some info (so not as good for TRX files).
            // As with duration, there is room for some configuration of naming strategy at some point.
            if (test.Cases.Count > 1 && testCase.Assertions.Count > 1)
            {
                return string.IsNullOrEmpty(testCase.Description)
                    ? assertion.Description
                    : $"{assertion.Description} for test case {testCase.Description}"; // TODO-LOCALISATION: localisation needed if this ever catches on
            }
            else if (test.Cases.Count > 1)
            {
                return testCase.Description;
            }
            else if (testCase.Assertions.Count > 1)
            {
                return assertion.Description;
            }
            else
            {
                // TODO-MAINTAINABILITY: this is bad - VSTest-specific behaviour. Sort this out, ideally by changing the ITestContainer interface to talk unequivocally in FlUnit terms,
                // which'll mean that the adapter has a bit more work to do, but.. Sort this out BEFORE pulling common execution logic out of VSTest adapter.
                return null; 
            }
        }
    }
}
