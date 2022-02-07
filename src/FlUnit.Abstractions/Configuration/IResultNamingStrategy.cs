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
        /// TODO-MAINTAINABILITY: Think about how this overlaps with itestcontainer interface. if interface there is changed
        /// to make adapter more responsible for mapping flunit to test platform, how is this affected?
        /// think about visual studio vs trx. test out dotnet test again..
        /// </remarks>
        string GetResultName(Test test, ITestCase testCase, ITestAssertion assertion);
    }
}
