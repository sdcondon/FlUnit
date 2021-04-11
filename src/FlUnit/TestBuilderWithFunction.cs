using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the first assertion for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<TResult>
    {
        private readonly Func<TResult> testFunction;

        internal TestBuilderWithFunction(
            Func<TResult> testFunction)
        {
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> Then(Expression<Action<TestFunctionOutcome<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> Then(
            Action<TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 1 "Given" clause
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<T1, TResult>
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> testFunction;

        internal TestBuilderWithFunction(
            Func<IEnumerable<T1>> arrange,
            Func<T1, TResult> testFunction)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> Then(Expression<Action<T1, TestFunctionOutcome<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> Then(
            Action<T1, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<T1, T2, TResult>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> testFunction;

        internal TestBuilderWithFunction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> testFunction)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> Then(Expression<Action<T1, T2, TestFunctionOutcome<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> Then(
            Action<T1, T2, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 3 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<T1, T2, T3, TResult>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> testFunction;

        internal TestBuilderWithFunction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> testFunction)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> Then(Expression<Action<T1, T2, T3, TestFunctionOutcome<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> Then(
            Action<T1, T2, T3, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(assertion, description));
        }
    }
}