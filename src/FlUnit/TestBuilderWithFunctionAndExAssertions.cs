using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
#if NET6_0
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value - but is expected to throw an exception.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndExAssertions<TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<TResult> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal TestBuilderWithFunctionAndExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<TResult> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndExAssertions{TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndExAssertions<TResult> builder)
        {
            return new FunctionTest<TResult>(
                builder.configurationOverrides,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<TResult>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public TestBuilderWithFunctionAndExAssertions<TResult> And(
            Action<Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<TResult> And(Expression<Action<Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<TResult> And(Action<Exception> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<Exception> assert;

            internal AssertionImpl(Action<Exception> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<Exception>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal void Invoke(TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 1 "Given" clause
    /// and for which the "When" clause returns a value - but is expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndExAssertions<T1, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal TestBuilderWithFunctionAndExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<IEnumerable<T1>> arrange,
            Func<T1, TResult> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndExAssertions{T1, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndExAssertions<T1, TResult> builder)
        {
            return new FunctionTest<T1, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, TResult>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> And(
            Action<T1, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> And(Expression<Action<T1, Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> And(Action<T1, Exception> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, Exception> assert;

            internal AssertionImpl(Action<T1, Exception> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, Exception>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal void Invoke(T1 a1, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value - but is expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndExAssertions<T1, T2, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal TestBuilderWithFunctionAndExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndExAssertions{T1, T2, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> builder)
        {
            return new FunctionTest<T1, T2, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, TResult>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> And(
            Action<T1, T2, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> And(Expression<Action<T1, T2, Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> And(Action<T1, T2, Exception> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, Exception> assert;

            internal AssertionImpl(Action<T1, T2, Exception> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, Exception>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal void Invoke(T1 a1, T2 a2, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, a2, outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 3 "Given" clauses
    /// and for which the "When" clause returns a value - but is expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal TestBuilderWithFunctionAndExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> testFunction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndExAssertions{T1, T2, T3, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> builder)
        {
            return new FunctionTest<T1, T2, T3, TResult>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new FunctionTest<T1, T2, T3, TResult>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> And(
            Action<T1, T2, T3, Exception> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> And(Expression<Action<T1, T2, T3, Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> And(Action<T1, T2, T3, Exception> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3, Exception> assert;

            internal AssertionImpl(Action<T1, T2, T3, Exception> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3, Exception>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal void Invoke(T1 a1, T2 a2, T3 a3, TestFunctionOutcome<TResult> outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, a2, a3, outcome.Exception);
            }
        }
    }
}