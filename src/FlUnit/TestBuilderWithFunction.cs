using System;
using System.Linq.Expressions;

namespace FlUnit
{
    //// TODO: T4 template me to eliminate repetition and allow for more prereqs with no effort

    /// <summary>
    /// Builder for providing the first assertion for a test with no "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<TResult>
    {
        private readonly Func<TResult> testFunction;

        internal TestBuilderWithFunction(Func<TResult> testFunction)
        {
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> Then(Expression<Action<TestFunctionResult<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(testFunction, new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> Then(Action<TestFunctionResult<TResult>> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(testFunction, new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with one "Given" clause
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the test pre-requisite.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<T1, TResult>
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;

        internal TestBuilderWithFunction(T1 prereq, Func<T1, TResult> testFunction)
        {
            this.prereq = prereq;
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> Then(Expression<Action<T1, TestFunctionResult<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(prereq, testFunction, new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> Then(Action<T1, TestFunctionResult<TResult>> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(prereq, testFunction, new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with two "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first test pre-requisite.</typeparam>
    /// <typeparam name="T2">The type of the second test pre-requisite.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<T1, T2, TResult>
    {
        private readonly (T1, T2) prereqs;
        private readonly Func<T1, T2, TResult> testFunction;

        internal TestBuilderWithFunction((T1, T2) prereqs, Func<T1, T2, TResult> testFunction)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> Then(Expression<Action<T1, T2, TestFunctionResult<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(prereqs, testFunction, new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> Then(Action<T1, T2, TestFunctionResult<TResult>> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(prereqs, testFunction, new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(assertion, description));
        }
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with two "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the first test pre-requisite.</typeparam>
    /// <typeparam name="T2">The type of the second test pre-requisite.</typeparam>
    /// <typeparam name="T3">The type of the third test pre-requisite.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunction<T1, T2, T3, TResult>
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Func<T1, T2, T3, TResult> testFunction;

        internal TestBuilderWithFunction((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> Then(Expression<Action<T1, T2, T3, TestFunctionResult<TResult>>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(prereqs, testFunction, new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> Then(Action<T1, T2, T3, TestFunctionResult<TResult>> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(prereqs, testFunction, new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(assertion, description));
        }
    }
}
