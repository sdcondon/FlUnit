﻿namespace FlUnit
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
        /// Invokes the assertion.
        /// </summary>
        void Invoke();
    }
}
