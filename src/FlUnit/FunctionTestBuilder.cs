using FlUnit.Configuration;
using System;
using System.Collections.Generic;
#if NET6_0
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the first assertion for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilder<TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<TResult> testFunction;

        internal FunctionTestBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<TResult> testFunction)
        {
            this.configurationOverrides = configurationOverrides;
            this.testFunction = testFunction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> Then(
            Action<TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new FunctionTestBuilderWithAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithAssertions<TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> Then(
            Expression<Action<TestFunctionOutcome<TResult>>> assertion)
        {
            return new FunctionTestBuilderWithAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithAssertions<TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> Then(
            Action<TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new FunctionTestBuilderWithAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithAssertions<TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> ThenReturns()
        {
            return new FunctionTestBuilderWithRVAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> ThenReturns(string description)
        {
            return new FunctionTestBuilderWithRVAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> ThenReturns(
            Action<TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithRVAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> ThenReturns(
            Expression<Action<TResult>> assertion)
        {
            return new FunctionTestBuilderWithRVAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> ThenReturns(
            Action<TResult> assertion,
            string description)
        {
            return new FunctionTestBuilderWithRVAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<TResult> ThenThrows()
        {
            return new FunctionTestBuilderWithExAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithExAssertions<TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<TResult> ThenThrows(string description)
        {
            return new FunctionTestBuilderWithExAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithExAssertions<TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<TResult> ThenThrows(
            Action<Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithExAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithExAssertions<TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<TResult> ThenThrows(
            Expression<Action<Exception>> assertion)
        {
            return new FunctionTestBuilderWithExAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithExAssertions<TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<TResult> ThenThrows(
            Action<Exception> assertion,
            string description)
        {
            return new FunctionTestBuilderWithExAssertions<TResult>(
                configurationOverrides,
                testFunction,
                new FunctionTestBuilderWithExAssertions<TResult>.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 1 "Given" clause
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilder<T1, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> testFunction;

        internal FunctionTestBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, IEnumerable<T1>> arrange,
            Func<T1, TResult> testFunction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> Then(
            Action<T1, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new FunctionTestBuilderWithAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> Then(
            Expression<Action<T1, TestFunctionOutcome<TResult>>> assertion)
        {
            return new FunctionTestBuilderWithAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> Then(
            Action<T1, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new FunctionTestBuilderWithAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> ThenReturns()
        {
            return new FunctionTestBuilderWithRVAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> ThenReturns(string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> ThenReturns(
            Action<T1, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> ThenReturns(
            Expression<Action<T1, TResult>> assertion)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> ThenReturns(
            Action<T1, TResult> assertion,
            string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, TResult> ThenThrows()
        {
            return new FunctionTestBuilderWithExAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, TResult> ThenThrows(string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, TResult> ThenThrows(
            Action<T1, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithExAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, TResult> ThenThrows(
            Expression<Action<T1, Exception>> assertion)
        {
            return new FunctionTestBuilderWithExAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, TResult> ThenThrows(
            Action<T1, Exception> assertion,
            string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, TResult>.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilder<T1, T2, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> testFunction;

        internal FunctionTestBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> testFunction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> Then(
            Action<T1, T2, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> Then(
            Expression<Action<T1, T2, TestFunctionOutcome<TResult>>> assertion)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> Then(
            Action<T1, T2, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> ThenReturns()
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> ThenReturns(string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> ThenReturns(
            Action<T1, T2, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> ThenReturns(
            Expression<Action<T1, T2, TResult>> assertion)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> ThenReturns(
            Action<T1, T2, TResult> assertion,
            string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, TResult> ThenThrows()
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, TResult> ThenThrows(string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, TResult> ThenThrows(
            Action<T1, T2, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, TResult> ThenThrows(
            Expression<Action<T1, T2, Exception>> assertion)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, TResult> ThenThrows(
            Action<T1, T2, Exception> assertion,
            string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, TResult>.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 3 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilder<T1, T2, T3, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> testFunction;

        internal FunctionTestBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> testFunction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> Then(
            Action<T1, T2, T3, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> Then(
            Expression<Action<T1, T2, T3, TestFunctionOutcome<TResult>>> assertion)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> Then(
            Action<T1, T2, T3, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> ThenReturns()
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> ThenReturns(string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> ThenReturns(
            Action<T1, T2, T3, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> ThenReturns(
            Expression<Action<T1, T2, T3, TResult>> assertion)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> ThenReturns(
            Action<T1, T2, T3, TResult> assertion,
            string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult> ThenThrows()
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult> ThenThrows(string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult> ThenThrows(
            Action<T1, T2, T3, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult> ThenThrows(
            Expression<Action<T1, T2, T3, Exception>> assertion)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult> ThenThrows(
            Action<T1, T2, T3, Exception> assertion,
            string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, TResult>.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 4 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilder<T1, T2, T3, T4, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>) arrange;
        private readonly Func<T1, T2, T3, T4, TResult> testFunction;

        internal FunctionTestBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>) arrange,
            Func<T1, T2, T3, T4, TResult> testFunction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> Then(
            Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> Then(
            Expression<Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>>> assertion)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> Then(
            Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> ThenReturns()
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> ThenReturns(string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> ThenReturns(
            Action<T1, T2, T3, T4, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> ThenReturns(
            Expression<Action<T1, T2, T3, T4, TResult>> assertion)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> ThenReturns(
            Action<T1, T2, T3, T4, TResult> assertion,
            string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult> ThenThrows()
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult> ThenThrows(string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult> ThenThrows(
            Action<T1, T2, T3, T4, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult> ThenThrows(
            Expression<Action<T1, T2, T3, T4, Exception>> assertion)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult> ThenThrows(
            Action<T1, T2, T3, T4, Exception> assertion,
            string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, TResult>.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 5 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilder<T1, T2, T3, T4, T5, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>, Func<ITestContext, IEnumerable<T5>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, TResult> testFunction;

        internal FunctionTestBuilder(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>, Func<ITestContext, IEnumerable<T5>>) arrange,
            Func<T1, T2, T3, T4, T5, TResult> testFunction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> Then(
            Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> Then(
            Expression<Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>>> assertion)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// <para/>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> Then(
            Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            return new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> ThenReturns()
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully. Specifically, adds an assertion that simply verifies that the test function returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> ThenReturns(string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> ThenReturns(
            Action<T1, T2, T3, T4, T5, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> ThenReturns(
            Expression<Action<T1, T2, T3, T4, T5, TResult>> assertion)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> ThenReturns(
            Action<T1, T2, T3, T4, T5, TResult> assertion,
            string description)
        {
            return new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult> ThenThrows()
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(Messages.ImplicitAssertionTestFunctionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test function threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult> ThenThrows(string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult> ThenThrows(
            Action<T1, T2, T3, T4, T5, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")]string assertionExpression = null)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(
                    assertion,
                    description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult> ThenThrows(
            Expression<Action<T1, T2, T3, T4, T5, Exception>> assertion)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test function is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult> ThenThrows(
            Action<T1, T2, T3, T4, T5, Exception> assertion,
            string description)
        {
            return new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>(
                configurationOverrides,
                arrange,
                testFunction,
                new FunctionTestBuilderWithExAssertions<T1, T2, T3, T4, T5, TResult>.AssertionImpl(assertion, description));
        }
#endif
    }
}