﻿using FlUnit.Configuration;
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
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithAction
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action testAction;

        internal TestBuilderWithAction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Action testAction)
        {
            this.configurationOverrides = configurationOverrides;
            this.testAction = testAction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions Then(
            Action<TestActionOutcome> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndAssertions.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions Then(Expression<Action<TestActionOutcome>> assertion)
        {
            return new TestBuilderWithActionAndAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndAssertions.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions Then(Action<TestActionOutcome> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndAssertions.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions ThenReturns()
        {
            return new TestBuilderWithActionAndRVAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndRVAssertions.AssertionImpl(Messages.ImplicitAssertionTestActionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions ThenReturns(string description)
        {
            return new TestBuilderWithActionAndRVAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndRVAssertions.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions ThenReturns(
            Action assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndRVAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndRVAssertions.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions ThenReturns(Expression<Action> assertion)
        {
            return new TestBuilderWithActionAndRVAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndRVAssertions.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions ThenReturns(Action assertion, string description)
        {
            return new TestBuilderWithActionAndRVAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndRVAssertions.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows()
        {
            return new TestBuilderWithActionAndExAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndExAssertions.AssertionImpl(Messages.ImplicitAssertionTestActionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows(string description)
        {
            return new TestBuilderWithActionAndExAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndExAssertions.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows(
            Action<Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndExAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndExAssertions.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows(Expression<Action<Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndExAssertions.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions ThenThrows(Action<Exception> assertion, string description)
        {
            return new TestBuilderWithActionAndExAssertions(
                configurationOverrides,
                testAction,
                new TestBuilderWithActionAndExAssertions.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 1 "Given" clause
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithAction<T1>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;

        internal TestBuilderWithAction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<IEnumerable<T1>> arrange,
            Action<T1> testAction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1> Then(
            Action<T1, TestActionOutcome> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1> Then(Expression<Action<T1, TestActionOutcome>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1> Then(Action<T1, TestActionOutcome> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1> ThenReturns()
        {
            return new TestBuilderWithActionAndRVAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1>.AssertionImpl(Messages.ImplicitAssertionTestActionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1> ThenReturns(string description)
        {
            return new TestBuilderWithActionAndRVAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1> ThenReturns(
            Action<T1> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndRVAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1> ThenReturns(Expression<Action<T1>> assertion)
        {
            return new TestBuilderWithActionAndRVAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1> ThenReturns(Action<T1> assertion, string description)
        {
            return new TestBuilderWithActionAndRVAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows()
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.AssertionImpl(Messages.ImplicitAssertionTestActionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows(string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows(
            Action<T1, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows(Expression<Action<T1, Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> ThenThrows(Action<T1, Exception> assertion, string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1>.AssertionImpl(assertion, description));
        }
#endif
    }

    /// <summary>
    /// Builder for providing the first assertion for a test with 2 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithAction<T1, T2>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;

        internal TestBuilderWithAction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(
            Action<T1, T2, TestActionOutcome> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(Expression<Action<T1, T2, TestActionOutcome>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1, T2> Then(Action<T1, T2, TestActionOutcome> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2> ThenReturns()
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2>.AssertionImpl(Messages.ImplicitAssertionTestActionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2> ThenReturns(string description)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2> ThenReturns(
            Action<T1, T2> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2> ThenReturns(Expression<Action<T1, T2>> assertion)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2> ThenReturns(Action<T1, T2> assertion, string description)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows()
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.AssertionImpl(Messages.ImplicitAssertionTestActionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows(string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows(
            Action<T1, T2, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows(Expression<Action<T1, T2, Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> ThenThrows(Action<T1, T2, Exception> assertion, string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2>.AssertionImpl(assertion, description));
        }
#endif
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
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;

        internal TestBuilderWithAction(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(
            Action<T1, T2, T3, TestActionOutcome> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2, T3>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(Expression<Action<T1, T2, T3, TestActionOutcome>> assertion)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2, T3>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        /// <remarks>
        /// NB: Most of the time <see cref="ThenReturns()"/> or <see cref="ThenThrows()"/> (or overloads thereof) are better choices.
        /// This method exists only to facilitate tests with multiple cases; some of which are expected to return successfully, others not.
        /// </remarks>
        public TestBuilderWithActionAndAssertions<T1, T2, T3> Then(Action<T1, T2, T3, TestActionOutcome> assertion, string description)
        {
            return new TestBuilderWithActionAndAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndAssertions<T1, T2, T3>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> ThenReturns()
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2, T3>.AssertionImpl(Messages.ImplicitAssertionTestActionShouldReturn));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully. Specifically, adds an assertion that simply verifies that the test action returned successfully.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> ThenReturns(string description)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2, T3>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> ThenReturns(
            Action<T1, T2, T3> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2, T3>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> ThenReturns(Expression<Action<T1, T2, T3>> assertion)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2, T3>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to return successfully.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> ThenReturns(Action<T1, T2, T3> assertion, string description)
        {
            return new TestBuilderWithActionAndRVAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndRVAssertions<T1, T2, T3>.AssertionImpl(assertion, description));
        }
#endif

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows()
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.AssertionImpl(Messages.ImplicitAssertionTestActionShouldThrow));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception. Specifically, adds an assertion that simply verifies that the test action threw an exception.
        /// </summary>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows(string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.AssertionImpl(description));
        }

#if NET6_0
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows(
            Action<T1, T2, T3, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
        }
#else
        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows(Expression<Action<T1, T2, T3, Exception>> assertion)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.AssertionImpl(assertion));
        }

        /// <summary>
        /// Adds the first assertion for the test if the test action is expected to throw an exception.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> ThenThrows(Action<T1, T2, T3, Exception> assertion, string description)
        {
            return new TestBuilderWithActionAndExAssertions<T1, T2, T3>(
                configurationOverrides,
                arrange,
                testAction,
                new TestBuilderWithActionAndExAssertions<T1, T2, T3>.AssertionImpl(assertion, description));
        }
#endif
    }
}