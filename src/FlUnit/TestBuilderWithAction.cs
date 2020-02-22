using System;
using System.Linq.Expressions;

namespace FlUnit
{
    //// TODO: T4 template me!!

    /// <summary>
    /// Builder for providing the first assertion for a test with no "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithAction
    {
        private readonly Action testAction;

        internal TestBuilderWithAction(Action testAction)
        {
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions Then(Expression<Action<TestActionResult>> assertion)
        {
            return new TestBuilderWithActionAndAssertions(testAction, new TestBuilderWithActionAndAssertions.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions Then(Action<TestActionResult> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions(testAction, new TestBuilderWithActionAndAssertions.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with one "Given" clause
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first pre-requisite of the test.</typeparam>
    public sealed class TestBuilderWithAction<T1>
    {
        private readonly T1 prereq;
        private readonly Action<T1> testAction;

        internal TestBuilderWithAction(T1 prereq, Action<T1> testAction)
        {
            this.prereq = prereq;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1> Then(Expression<Action<T1, TestActionResult>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1>(prereq, testAction, new TestBuilderWithActionAndAssertions<T1>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1> Then(Action<T1, TestActionResult> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1>(prereq, testAction, new TestBuilderWithActionAndAssertions<T1>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with two "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithAction<T1, T2>
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> testAction;

        internal TestBuilderWithAction((T1, T2) prereqs, Action<T1, T2> testAction)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(Expression<Action<T1, T2, TestActionResult>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(prereqs, testAction, new TestBuilderWithActionAndAssertions<T1, T2>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(Action<T1, T2, TestActionResult> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(prereqs, testAction, new TestBuilderWithActionAndAssertions<T1, T2>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with three "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithAction<T1, T2, T3>
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> testAction;

        internal TestBuilderWithAction((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(Expression<Action<T1, T2, T3, TestActionResult>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(prereqs, testAction, new TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(Action<T1, T2, T3, TestActionResult> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(prereqs, testAction, new TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion(assertion, description));
        }
    }
}
