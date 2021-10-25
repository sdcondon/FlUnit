﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing additional assertions for a test with a 0 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndExAssertions
    {
        private readonly Action testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndExAssertions(
            Action testAction,
            Assertion assertion)
        {
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndExAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndExAssertions builder)
        {
            return new TestAction(
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions And(Expression<Action<Exception>> assertion)
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
        public TestBuilderWithActionAndExAssertions And(Action<Exception> assertion, string description)
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

            public Action<TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 1 "Given" clause
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithActionAndExAssertions<T1>
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndExAssertions(
            Func<IEnumerable<T1>> arrange,
            Action<T1> testAction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndExAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndExAssertions<T1> builder)
        {
            return new TestAction<T1>(
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction<T1>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1> And(Expression<Action<T1, Exception>> assertion)
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
        public TestBuilderWithActionAndExAssertions<T1> And(Action<T1, Exception> assertion, string description)
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

            public Action<T1, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 2 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithActionAndExAssertions<T1, T2>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndExAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndExAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndExAssertions<T1, T2> builder)
        {
            return new TestAction<T1, T2>(
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction<T1, T2>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2> And(Expression<Action<T1, T2, Exception>> assertion)
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
        public TestBuilderWithActionAndExAssertions<T1, T2> And(Action<T1, T2, Exception> assertion, string description)
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

            public Action<T1, T2, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 3 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithActionAndExAssertions<T1, T2, T3>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndExAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndExAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndExAssertions<T1, T2, T3> builder)
        {
            return new TestAction<T1, T2, T3>(
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction<T1, T2, T3>.Case.Assertion(tc, a.Action, a.Description)));
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, Exception>> assertion)
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
        public TestBuilderWithActionAndExAssertions<T1, T2, T3> And(Action<T1, T2, T3, Exception> assertion, string description)
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

            public Action<T1, T2, T3, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }
}
