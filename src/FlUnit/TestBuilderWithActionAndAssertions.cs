using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlUnit
{
    //// TODO: T4 template me!!

    /// <summary>
    /// Builder for providing additional assertions for a test with no "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndAssertions
    {
        private readonly Action testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(Action testAction, Assertion assertion)
        {
            this.testAction = testAction;
            assertions.Add(assertion);
        }
        
        public static implicit operator Test(TestBuilderWithActionAndAssertions builder)
        {
            return new TestAction(builder.testAction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions And(Expression<Action<TestActionResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        public TestBuilderWithActionAndAssertions And(Action<TestActionResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<TestActionResult> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<TestActionResult>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<TestActionResult> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a single "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndAssertions<T1>
    {
        private readonly T1 prereq;
        private readonly Action<T1> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(T1 prereq, Action<T1> testAction, Assertion assertion)
        {
            this.prereq = prereq;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1> buillder)
        {
            return new TestAction<T1>(buillder.prereq, buillder.testAction, buillder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1> And(Expression<Action<T1, TestActionResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        public TestBuilderWithActionAndAssertions<T1> And(Action<T1, TestActionResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, TestActionResult> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<T1, TestActionResult>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<T1, TestActionResult> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a two "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndAssertions<T1, T2>
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions((T1, T2) prereqs, Action<T1, T2> testAction, Assertion assertion)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1, T2> builder)
        {
            return new TestAction<T1, T2>(builder.prereqs, builder.testAction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2> And(Expression<Action<T1, T2, TestActionResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        public TestBuilderWithActionAndAssertions<T1, T2> And(Action<T1, T2, TestActionResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, TestActionResult> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, TestActionResult>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, TestActionResult> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a three "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndAssertions<T1, T2, T3>
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction, Assertion assertion)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1, T2, T3> builder)
        {
            return new TestAction<T1, T2, T3>(builder.prereqs, builder.testAction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, TestActionResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(Action<T1, T2, T3, TestActionResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3, TestActionResult> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, T3, TestActionResult>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, T3, TestActionResult> Action { get; }

            public string Description { get; }
        }
    }
}
