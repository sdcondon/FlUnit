﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing additional assertions for a test with a 0 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndAssertions
    {
        private readonly Action testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
            Action testAction,
            Assertion assertion)
        {
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions builder)
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
        public TestBuilderWithActionAndAssertions And(Expression<Action> assertion)
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
        public TestBuilderWithActionAndAssertions And(Action assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action action, string description)
            {
                Action = (outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    action();
                };
                Description = description;
            }

            internal Assertion(Expression<Action> expression)
            {
                Action = (outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    expression.Compile()();
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
    public sealed class TestBuilderWithActionAndAssertions<T1>
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
            Func<IEnumerable<T1>> arrange,
            Action<T1> testAction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1> builder)
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
        public TestBuilderWithActionAndAssertions<T1> And(Expression<Action<T1>> assertion)
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
        public TestBuilderWithActionAndAssertions<T1> And(Action<T1> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1> action, string description)
            {
                Action = (a, outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    action(a);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1>> expression)
            {
                Action = (a, outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    expression.Compile()(a);
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
    public sealed class TestBuilderWithActionAndAssertions<T1, T2>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1, T2> builder)
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
        public TestBuilderWithActionAndAssertions<T1, T2> And(Expression<Action<T1, T2>> assertion)
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
        public TestBuilderWithActionAndAssertions<T1, T2> And(Action<T1, T2> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2> action, string description)
            {
                Action = (a1, a2, outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    action(a1, a2);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2>> expression)
            {
                Action = (a1, a2, outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    expression.Compile()(a1, a2);
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
    public sealed class TestBuilderWithActionAndAssertions<T1, T2, T3>
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction,
            Assertion assertion)
        {
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1, T2, T3> builder)
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
        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3>> assertion)
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
        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(Action<T1, T2, T3> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3> action, string description)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    action(a1, a2, a3);
                };
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, T3>> expression)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    if (outcome.Exception != null)
                    {
                        ExceptionDispatchInfo.Capture(outcome.Exception).Throw();
                    }

                    expression.Compile()(a1, a2, a3);
                };
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, T3, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }
}
