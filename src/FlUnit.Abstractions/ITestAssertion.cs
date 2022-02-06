namespace FlUnit
{
    /// <summary>
    /// Interface for types representing a invocable assertion for a test.
    /// </summary>
    public interface ITestAssertion
    {
        /// <summary>
        /// Gets the description of this assertion.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Tests the assertion. Failures should be indicated by thrown exceptions (ideally those implementing <see cref="ITestFailureDetails"/>).
        /// </summary>
        /// <remarks>
        /// While the verb used in the summary here is "test", we don't want to overload the term "test"
        /// when talking about FlUnit tests, and using "Assert" follows the pattern of <see cref="Test"/>s
        /// having <see cref="Test.Arrange"/>, and <see cref="ITestCase"/>s having <see cref="ITestCase.Act"/>.
        /// Which is nice.
        /// </remarks>
        void Assert();
    }
}
