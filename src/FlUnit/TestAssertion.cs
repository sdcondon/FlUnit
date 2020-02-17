using System;
using System.Linq.Expressions;

namespace FlUnit
{
    /// <summary>
    /// Container for the details of an assertion for a test.
    /// </summary>
    public class TestAssertion
    {
        private readonly Action action;

        internal TestAssertion(Action action, string description)
        {
            this.action = action;
            Description = description;
        }

        internal TestAssertion(Expression<Action> expression)
        {
            this.action = expression.Compile();
            Description = expression.Body.ToString();
        }

        /// <summary>
        /// Gets the description of this assertion.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Invokes the assertion.
        /// </summary>
        public void Invoke() => action.Invoke();
    }
}
