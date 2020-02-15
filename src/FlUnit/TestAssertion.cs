using System;
using System.Linq.Expressions;

namespace FlUnit
{
    public class TestAssertion
    {
        internal TestAssertion(Action action, string description)
        {
            Action = action;
            Description = description;
        }

        internal TestAssertion(Expression<Action> expression)
        {
            Action = expression.Compile();
            Description = expression.Body.ToString();
        }

        /// <summary>
        /// Gets the assertion action to be invoked.
        /// </summary>
        public Action Action { get; }

        /// <summary>
        /// Gets the description of this assertion.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Invokes the assertion.
        /// </summary>
        public void Invoke() => Action.Invoke();
    }
}
