﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause returns a value - but is expected to throw an exception.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndExAssertions<TResult>
    {
        private readonly Func<TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndExAssertions(
            Func<TResult> testFunction,
            Assertion assertion)
        {
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithFunctionAndExAssertions{TResult}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithFunctionAndExAssertions<TResult> builder)
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
        public TestBuilderWithFunctionAndExAssertions<TResult> And(Expression<Action<Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<TResult> And(Action<Exception> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<Exception> action, string description)
            {
                Action = (outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one");
                    }

                    action(outcome.Exception);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<Exception>> expression)
            {
                Action = (outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one", outcome.Exception);
                    }

                    expression.Compile()(outcome.Exception);
                };
                Description = expression.Body.ToString();
            }

            public Action<TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
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
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndExAssertions(
            Func<IEnumerable<T1>> arrange,
            Func<T1, TResult> testFunction,
            Assertion assertion)
        {
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
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> And(Expression<Action<T1, Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<T1, TResult> And(Action<T1, Exception> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, Exception> action, string description)
            {
                Action = (a, outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one");
                    }

                    action(a, outcome.Exception);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, Exception>> expression)
            {
                Action = (a, outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one", outcome.Exception);
                    }

                    expression.Compile()(a, outcome.Exception);
                };
                Description = expression.Body.ToString();
            }

            public Action<T1, TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
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
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndExAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> testFunction,
            Assertion assertion)
        {
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> And(Expression<Action<T1, T2, Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, TResult> And(Action<T1, T2, Exception> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, Exception> action, string description)
            {
                Action = (a1, a2, outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one");
                    }

                    action(a1, a2, outcome.Exception);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, Exception>> expression)
            {
                Action = (a1, a2, outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one", outcome.Exception);
                    }

                    expression.Compile()(a1, a2, outcome.Exception);
                };
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
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
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndExAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> testFunction,
            Assertion assertion)
        {
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> And(Expression<Action<T1, T2, T3, Exception>> assertion)
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
        public TestBuilderWithFunctionAndExAssertions<T1, T2, T3, TResult> And(Action<T1, T2, T3, Exception> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3, Exception> action, string description)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one");
                    }

                    action(a1, a2, a3, outcome.Exception);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, T3, Exception>> expression)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    if (outcome.Exception == null)
                    {
                        throw new Exception("An exception was expected but the When clause didn't throw one", outcome.Exception);
                    }

                    expression.Compile()(a1, a2, a3, outcome.Exception);
                };
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, T3, TestFunctionOutcome<TResult>> Action { get; }

            public string Description { get; }
        }
    }
}