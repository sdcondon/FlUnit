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
        /// Invokes the assertion. Failures are indicated by thrown exceptions.
        /// </summary>
        /// <remarks>
        /// TODO-MAINTAINABILITY: do you "invoke" an assertion? Or do you "test" it? hmm, don't want to overload the word "test".
        /// maybe you "assert" it - which would complement the "arrange" method in test and the "act" method in itestcase..
        /// </remarks>
        void Invoke();
    }
}
