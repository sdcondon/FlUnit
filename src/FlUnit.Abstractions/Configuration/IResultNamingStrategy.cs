namespace FlUnit.Configuration
{
    /// <summary>
    /// Interface for types that provide a strategy for labelling individual FlUnit test results;
    /// </summary>
    public interface IResultNamingStrategy
    {
        /// <summary>
        /// Gets the display name to be applied to a particular a test result.
        /// </summary>
        /// <param name="test">The test for which the result to name is.</param>
        /// <param name="testCase">The test case for which the result to name is.</param>
        /// <param name="assertion">The assertion for which the result to name is.</param>
        /// <returns>The display name of the result.</returns>
        /// <remarks>
        /// TODO-MAINTAINABILITY: Need to think about how this is related to the ITestContainer interface.
        /// If that interface there is changed to make adapters more responsible for mapping FlUnit
        /// concepts to the test platform, how is this affected?
        /// E.g. Think about visual studio vs trx. Need to test out dotnet test again.
        /// </remarks>
        string GetResultName(Test test, ITestCase testCase, ITestAssertion assertion);
    }
}
