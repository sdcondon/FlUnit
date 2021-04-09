namespace FlUnit
{
    /// <summary>
    /// Abstraction for types representing a invocable assertion for a test.
    /// </summary>
    public abstract class TestAssertion
    {
        /// <summary>
        /// Gets the description of this assertion.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Invokes the assertion.
        /// </summary>
        public abstract void Invoke();
    }
}
