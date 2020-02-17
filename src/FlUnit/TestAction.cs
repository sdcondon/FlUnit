using System;
using System.Linq.Expressions;

namespace FlUnit
{
    //// TODO: T4 template me!!

    /// <summary>
    /// Builder for providing the first assertion for a test with no "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestAction
    {
        private readonly Action testAction;

        internal TestAction(Action testAction)
        {
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions Then(Expression<Action<TestActionResult>> assertion)
        {
            return new TestActionAndAssertions(testAction, assertion);
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions Then(Action<TestActionResult> assertion, string description)
        {
            return new TestActionAndAssertions(testAction, assertion, description);
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with one "Given" clause
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first pre-requisite of the test.</typeparam>
    public class TestAction<T1>
    {
        private readonly T1 prereq;
        private readonly Action<T1> testAction;

        internal TestAction(T1 prereq, Action<T1> testAction)
        {
            this.prereq = prereq;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions<T1> Then(Expression<Action<T1, TestActionResult>> assertion)
        {
            return new TestActionAndAssertions<T1>(prereq, testAction, assertion);
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions<T1> Then(Action<T1, TestActionResult> assertion, string description)
        {
            return new TestActionAndAssertions<T1>(prereq, testAction, assertion, description);
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with two "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestAction<T1, T2>
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> testAction;

        internal TestAction((T1, T2) prereqs, Action<T1, T2> testAction)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions<T1, T2> Then(Expression<Action<T1, T2, TestActionResult>> assertion)
        {
            return new TestActionAndAssertions<T1, T2>(prereqs, testAction, assertion);
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions<T1, T2> Then(Action<T1, T2, TestActionResult> assertion, string description)
        {
            return new TestActionAndAssertions<T1, T2>(prereqs, testAction, assertion, description);
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with three "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestAction<T1, T2, T3>
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> testAction;

        internal TestAction((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions<T1, T2, T3> Then(Expression<Action<T1, T2, T3, TestActionResult>> assertion)
        {
            return new TestActionAndAssertions<T1, T2, T3>(prereqs, testAction, assertion);
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestActionAndAssertions<T1, T2, T3> Then(Action<T1, T2, T3, TestActionResult> assertion, string description)
        {
            return new TestActionAndAssertions<T1, T2, T3>(prereqs, testAction, assertion, description);
        }
    }
}
