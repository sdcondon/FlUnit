using System;

namespace FlUnit
{
    /// <summary>
    /// Interface for types representing a invocable assertion for a test.
    /// </summary>
    public interface ITestAssertion : IFormattable
    {
        /// <summary>
        /// Tests the assertion. Failures should be indicated by thrown exceptions (ideally those implementing <see cref="ITestFailureDetails"/>).
        /// </summary>
        void Assert();
    }
}
