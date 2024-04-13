using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
#if NET6_0
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif
using System.Threading.Tasks;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithAssertions<TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly Func<ValueTask<TResult>> testFunction;
#else
        private readonly Func<Task<TResult>> testFunction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            Func<ValueTask<TResult>> testFunction,
#else
            Func<Task<TResult>> testFunction,
#endif
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithAssertions{TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithAssertions<TResult> builder)
        {
            return new FunctionTest<TResult>(
                builder.configurationOverrides,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

#if NET6_0
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> And(
            Action<TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> AndAsync(
            Func<TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> And(
            Expression<Action<TestFunctionOutcome<TResult>>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> And(
            Action<TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<TResult> AndAsync(
            Func<TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<TestFunctionOutcome<TResult>, ValueTask> assert;

            internal AssertionImpl(Func<TestFunctionOutcome<TResult>, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<TestFunctionOutcome<TResult>, Task> assert;

            internal AssertionImpl(Func<TestFunctionOutcome<TResult>, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<TestFunctionOutcome<TResult>>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(TestFunctionOutcome<TResult> outcome) => assert(outcome);
#else
            internal Task Invoke(TestFunctionOutcome<TResult> outcome) => assert(outcome);
#endif
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 1 "Given" clause
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithAssertions<T1, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask<TResult>> testFunction;
#else
        private readonly Func<ITestContext, Task<IEnumerable<T1>>> arrange;
        private readonly Func<T1, Task<TResult>> testFunction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange,
            Func<T1, ValueTask<TResult>> testFunction,
#else
            Func<ITestContext, Task<IEnumerable<T1>>> arrange,
            Func<T1, Task<TResult>> testFunction,
#endif
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithAssertions{T1, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithAssertions<T1, TResult> builder)
        {
            return new FunctionTest<T1, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

#if NET6_0
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> And(
            Action<T1, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> AndAsync(
            Func<T1, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> And(
            Expression<Action<T1, TestFunctionOutcome<TResult>>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> And(
            Action<T1, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, TResult> AndAsync(
            Func<T1, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, TestFunctionOutcome<TResult>, ValueTask> assert;

            internal AssertionImpl(Func<T1, TestFunctionOutcome<TResult>, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, TestFunctionOutcome<TResult>, Task> assert;

            internal AssertionImpl(Func<T1, TestFunctionOutcome<TResult>, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, TestFunctionOutcome<TResult>>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, TestFunctionOutcome<TResult> outcome) => assert(a1, outcome);
#else
            internal Task Invoke(T1 a1, TestFunctionOutcome<TResult> outcome) => assert(a1, outcome);
#endif
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithAssertions<T1, T2, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask<TResult>> testFunction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, Task<TResult>> testFunction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange,
            Func<T1, T2, ValueTask<TResult>> testFunction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>) arrange,
            Func<T1, T2, Task<TResult>> testFunction,
#endif
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithAssertions{T1, T2, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithAssertions<T1, T2, TResult> builder)
        {
            return new FunctionTest<T1, T2, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

#if NET6_0
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> And(
            Action<T1, T2, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> AndAsync(
            Func<T1, T2, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> And(
            Expression<Action<T1, T2, TestFunctionOutcome<TResult>>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> And(
            Action<T1, T2, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, TResult> AndAsync(
            Func<T1, T2, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, TestFunctionOutcome<TResult>, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, TestFunctionOutcome<TResult>, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, TestFunctionOutcome<TResult>, Task> assert;

            internal AssertionImpl(Func<T1, T2, TestFunctionOutcome<TResult>, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, TestFunctionOutcome<TResult>>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, outcome);
#endif
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 3 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithAssertions<T1, T2, T3, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask<TResult>> testFunction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, Task<TResult>> testFunction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, ValueTask<TResult>> testFunction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, Task<TResult>> testFunction,
#endif
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithAssertions{T1, T2, T3, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

#if NET6_0
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> And(
            Action<T1, T2, T3, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> AndAsync(
            Func<T1, T2, T3, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> And(
            Expression<Action<T1, T2, T3, TestFunctionOutcome<TResult>>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> And(
            Action<T1, T2, T3, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, TResult> AndAsync(
            Func<T1, T2, T3, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, TestFunctionOutcome<TResult>, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, TestFunctionOutcome<TResult>, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, TestFunctionOutcome<TResult>, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, TestFunctionOutcome<TResult>, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, TestFunctionOutcome<TResult>>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, a3, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, a3, outcome);
#endif
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 4 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, ValueTask<TResult>> testFunction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, Task<TResult>> testFunction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, ValueTask<TResult>> testFunction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, Task<TResult>> testFunction,
#endif
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithAssertions{T1, T2, T3, T4, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, T4, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, T4, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

#if NET6_0
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> And(
            Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> AndAsync(
            Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> And(
            Expression<Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> And(
            Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, TResult> AndAsync(
            Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, TestFunctionOutcome<TResult>, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, TestFunctionOutcome<TResult>>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, a3, a4, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, a3, a4, outcome);
#endif
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 5 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, ValueTask<TResult>> testFunction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>, Func<ITestContext, Task<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, Task<TResult>> testFunction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal FunctionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, ValueTask<TResult>> testFunction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>, Func<ITestContext, Task<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, Task<TResult>> testFunction,
#endif
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="FunctionTestBuilderWithAssertions{T1, T2, T3, T4, T5, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, T4, T5, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, T4, T5, TResult>.Assertion(tc, a.Invoke, a.Description)));
        }

#if NET6_0
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> And(
            Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> AndAsync(
            Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> And(
            Expression<Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> And(
            Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>> assertion,
            string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public FunctionTestBuilderWithAssertions<T1, T2, T3, T4, T5, TResult> AndAsync(
            Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, Task> asyncAssertion,
            string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, T5, TestFunctionOutcome<TResult>>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, a3, a4, a5, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestFunctionOutcome<TResult> outcome) => assert(a1, a2, a3, a4, a5, outcome);
#endif
        }
    }
}