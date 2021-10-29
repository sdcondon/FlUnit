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
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> ThenReturns()
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndAssertions<TResult>.Assertion("Test function should return successfully")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> ThenReturns(string description)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> ThenReturns(Expression<Action<TResult>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> ThenReturns(Action<TResult> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndAssertions<TResult>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<TResult> ThenThrows()
        {
            return new TestBuilderWithFunctionAndExAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<TResult>.Assertion("Test function should throw an exception")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<TResult> ThenThrows(string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<TResult> ThenThrows(Expression<Action<Exception>> assertion)
        {
            return new TestBuilderWithFunctionAndExAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<TResult> ThenThrows(Action<Exception> assertion, string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<TResult>(
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<TResult>.Assertion(assertion, description));
        }

        //// NB: Hmmm, I think there is room for a little more here. Treading on assertion libraries toes a little, but it is probably enough of a special
        //// case that there is value: generic methods that assert that the exception thrown is of a particular type (and possibly even gives that type to the assertion)
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
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> ThenReturns()
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion("Test function should return successfully")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> ThenReturns(string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> ThenReturns(Expression<Action<T1, TResult>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> ThenReturns(Action<T1, TResult> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> ThenThrows()
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, TResult>.Assertion("Test function should throw an exception")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> ThenThrows(string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> ThenThrows(Expression<Action<T1, Exception>> assertion)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> ThenThrows(Action<T1, Exception> assertion, string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, TResult>.Assertion(assertion, description));
        }

        //// NB: Hmmm, I think there is room for a little more here. Treading on assertion libraries toes a little, but it is probably enough of a special
        //// case that there is value: generic methods that assert that the exception thrown is of a particular type (and possibly even gives that type to the assertion)
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
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> ThenReturns()
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion("Test function should return successfully")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> ThenReturns(string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> ThenReturns(Expression<Action<T1, T2, TResult>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> ThenReturns(Action<T1, T2, TResult> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> ThenThrows()
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>.Assertion("Test function should throw an exception")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> ThenThrows(string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> ThenThrows(Expression<Action<T1, T2, Exception>> assertion)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> ThenThrows(Action<T1, T2, Exception> assertion, string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>.Assertion(assertion, description));
        }

        //// NB: Hmmm, I think there is room for a little more here. Treading on assertion libraries toes a little, but it is probably enough of a special
        //// case that there is value: generic methods that assert that the exception thrown is of a particular type (and possibly even gives that type to the assertion)
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
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> ThenReturns()
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion("Test function should return successfully")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> ThenReturns(string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> ThenReturns(Expression<Action<T1, T2, T3, TResult>> assertion)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> ThenReturns(Action<T1, T2, T3, TResult> assertion, string description)
        {
            return new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion(assertion, description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> ThenThrows()
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>.Assertion("Test function should throw an exception")); // TODO-LOCALISATION: Localisation needed if this ever catches on
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> ThenThrows(string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>.Assertion(description));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> ThenThrows(Expression<Action<T1, T2, T3, Exception>> assertion)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>.Assertion(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> ThenThrows(Action<T1, T2, T3, Exception> assertion, string description)
        {
            return new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>(
                arrange,
                testFunction,
                new TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>.Assertion(assertion, description));
        }

        //// NB: Hmmm, I think there is room for a little more here. Treading on assertion libraries toes a little, but it is probably enough of a special
        //// case that there is value: generic methods that assert that the exception thrown is of a particular type (and possibly even gives that type to the assertion)
    }
}