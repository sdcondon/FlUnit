using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the first assertion for a test with 0 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithAction
    {
        private readonly Action testAction;

        internal TestBuilderWithAction(
            Action testAction)
        {
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions Then(Expression<Action> assertion)
        {
            return new TestBuilderWithActionAndAssertions(
                testAction,
                new TestBuilderWithActionAndAssertions.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions Then(Action assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions(
                testAction,
                new TestBuilderWithActionAndAssertions.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows(Expression<Action<Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions(
                testAction,
                new TestBuilderWithActionAndExAssertions.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows(
            Action<Exception> assertion,
            string description)
        {
            return new TestBuilderWithActionAndExAssertions(
                testAction,
                new TestBuilderWithActionAndExAssertions.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 1 "Given" clause
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithAction<T1>
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;

        internal TestBuilderWithAction(
            Func<IEnumerable<T1>> arrange,
            Action<T1> testAction)
        {
            this.arrange = arrange;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1> Then(Expression<Action<T1>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1>(
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1> Then(Action<T1> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1>(
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows(Expression<Action<T1, Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows(
            Action<T1, Exception> assertion,
            string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 2 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithAction<T1, T2>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;

        internal TestBuilderWithAction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction)
        {
            this.arrange = arrange;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(Expression<Action<T1, T2>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(Action<T1, T2> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows(Expression<Action<T1, T2, Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows(
            Action<T1, T2, Exception> assertion,
            string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 3 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithAction<T1, T2, T3>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;

        internal TestBuilderWithAction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction)
        {
            this.arrange = arrange;
            this.testAction = testAction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(Expression<Action<T1, T2, T3>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(Action<T1, T2, T3> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows(Expression<Action<T1, T2, T3, Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows(
            Action<T1, T2, T3, Exception> assertion,
            string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.Assertion(assertion, description));
        }
    }
}