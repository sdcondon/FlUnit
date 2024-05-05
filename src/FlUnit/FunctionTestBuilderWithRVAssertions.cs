using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
#if NET6_0
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithRVAssertions<TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ValueTask<TResult>> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ValueTask<TResult>> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithRVAssertions{TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithRVAssertions<TResult> builder)
        {
            return new FunctionTest<TResult>(
                builder.configurationOverrides,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> And(
            Action<TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<TResult> AndAsync(
            Func<TResult, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<TResult, ValueTask> assert;

            internal AssertionImpl(Func<TResult, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(outcome.Result) ?? ValueTask.CompletedTask;
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 1 "Given" clause
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithRVAssertions<T1, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask<TResult>> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange,
            Func<T1, ValueTask<TResult>> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithRVAssertions{T1, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithRVAssertions<T1, TResult> builder)
        {
            return new FunctionTest<T1, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> And(
            Action<T1, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, TResult> AndAsync(
            Func<T1, TResult, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, TResult, ValueTask> assert;

            internal AssertionImpl(Func<T1, TResult, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, outcome.Result) ?? ValueTask.CompletedTask;
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithRVAssertions<T1, T2, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask<TResult>> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange,
            Func<T1, T2, ValueTask<TResult>> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithRVAssertions{T1, T2, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithRVAssertions<T1, T2, TResult> builder)
        {
            return new FunctionTest<T1, T2, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> And(
            Action<T1, T2, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, TResult> AndAsync(
            Func<T1, T2, TResult, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, TResult, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, TResult, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, outcome.Result) ?? ValueTask.CompletedTask;
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 3 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask<TResult>> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, ValueTask<TResult>> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithRVAssertions{T1, T2, T3, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> And(
            Action<T1, T2, T3, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, TResult> AndAsync(
            Func<T1, T2, T3, TResult, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, T3, TResult, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, TResult, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, outcome.Result) ?? ValueTask.CompletedTask;
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 4 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, ValueTask<TResult>> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, ValueTask<TResult>> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithRVAssertions{T1, T2, T3, T4, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, T4, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, T4, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> And(
            Action<T1, T2, T3, T4, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, TResult> AndAsync(
            Func<T1, T2, T3, T4, TResult, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, T3, T4, TResult, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, TResult, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, a4, outcome.Result) ?? ValueTask.CompletedTask;
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 5 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, ValueTask<TResult>> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, ValueTask<TResult>> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithRVAssertions{T1, T2, T3, T4, T5, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, T4, T5, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, T4, T5, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> And(
            Action<T1, T2, T3, T4, T5, TResult> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the return value of the "When" clause.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5, TResult> AndAsync(
            Func<T1, T2, T3, T4, T5, TResult, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, T3, T4, T5, TResult, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, TResult, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, a4, a5, outcome.Result) ?? ValueTask.CompletedTask;
            }
        }
    }
}