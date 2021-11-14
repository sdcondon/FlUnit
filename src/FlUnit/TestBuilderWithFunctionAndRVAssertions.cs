using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndRVAssertions<TResult>
    {
        private readonly Func<TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndRVAssertions(
            Func<TResult> testFunction,
            Assertion assertion)
        {
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndRVAssertions{TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndRVAssertions<TResult> builder)
        {
            return new TestFunction<TResult>(
                builder.testFunction,
                tc => builder.assertions.Select(a => new TestFunction<TResult>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<TResult> And(Expression<Action<TResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<TResult> And(Action<TResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<TResult> action, string description)
            {
                Action = (outcome) =>
                {
                    outcome.ThrowIfException();
                    action(outcome.Result);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<TResult>> expression)
            {
                Action = (outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(outcome.Result);
                };
                Description = expression.Body.ToString();
            }

            internal Assertion(string description)
            {
                Action = (outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 1 "Given" clause
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndRVAssertions<T1, TResult>
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndRVAssertions(
            Func<IEnumerable<T1>> arrange,
            Func<T1, TResult> testFunction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndRVAssertions{T1, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndRVAssertions<T1, TResult> builder)
        {
            return new TestFunction<T1, TResult>(
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new TestFunction<T1, TResult>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<T1, TResult> And(Expression<Action<T1, TResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<T1, TResult> And(Action<T1, TResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, TResult> action, string description)
            {
                Action = (a, outcome) =>
                {
                    outcome.ThrowIfException();
                    action(a, outcome.Result);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, TResult>> expression)
            {
                Action = (a, outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(a, outcome.Result);
                };
                Description = expression.Body.ToString();
            }

            internal Assertion(string description)
            {
                Action = (a, outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<T1, TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value - and is expected to return successfully.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndRVAssertions<T1, T2, TResult>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndRVAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> testFunction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndRVAssertions{T1, T2, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndRVAssertions<T1, T2, TResult> builder)
        {
            return new TestFunction<T1, T2, TResult>(
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new TestFunction<T1, T2, TResult>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<T1, T2, TResult> And(Expression<Action<T1, T2, TResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<T1, T2, TResult> And(Action<T1, T2, TResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, TResult> action, string description)
            {
                Action = (a1, a2, outcome) =>
                {
                    outcome.ThrowIfException();
                    action(a1, a2, outcome.Result);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, TResult>> expression)
            {
                Action = (a1, a2, outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(a1, a2, outcome.Result);
                };
                Description = expression.Body.ToString();
            }

            internal Assertion(string description)
            {
                Action = (a1, a2, outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<T1, T2, TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
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
    public sealed class TestBuilderWithFunctionAndRVAssertions<T1, T2, T3, TResult>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndRVAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> testFunction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndRVAssertions{T1, T2, T3, TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndRVAssertions<T1, T2, T3, TResult> builder)
        {
            return new TestFunction<T1, T2, T3, TResult>(
                builder.arrange,
                builder.testFunction,
                tc => builder.assertions.Select(a => new TestFunction<T1, T2, T3, TResult>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<T1, T2, T3, TResult> And(Expression<Action<T1, T2, T3, TResult>> assertion)
        {
            assertions.Add(new Assertion(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndRVAssertions<T1, T2, T3, TResult> And(Action<T1, T2, T3, TResult> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3, TResult> action, string description)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    outcome.ThrowIfException();
                    action(a1, a2, a3, outcome.Result);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, T3, TResult>> expression)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(a1, a2, a3, outcome.Result);
                };
                Description = expression.Body.ToString();
            }

            internal Assertion(string description)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<T1, T2, T3, TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
        }
    }
}