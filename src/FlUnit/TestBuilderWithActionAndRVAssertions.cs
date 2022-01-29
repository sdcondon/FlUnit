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

namespace FlUnit
{
    /// <summary>
    /// Builder for providing additional assertions for a test with 0 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    public sealed class TestBuilderWithActionAndRVAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Action testAction,
            Assertion assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndRVAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndRVAssertions builder)
        {
            return new TestAction(
                builder.configurationOverrides,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction.Case.Assertion(tc, a.Action, a.Description)));
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
        public TestBuilderWithActionAndRVAssertions And(
            Action assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new Assertion(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions And(Expression<Action> assertion)
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
        public TestBuilderWithActionAndRVAssertions And(Action assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action action, string description)
            {
                Action = (outcome) =>
                {
                    outcome.ThrowIfException();
                    action();
                };
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action> expression)
            {
                Action = (outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()();
                };
                Description = expression.Body.ToString();
            }
#endif

            internal Assertion(string description)
            {
                Action = (outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 1 "Given" clause,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithActionAndRVAssertions<T1>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<IEnumerable<T1>> arrange,
            Action<T1> testAction,
            Assertion assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndRVAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndRVAssertions<T1> builder)
        {
            return new TestAction<T1>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction<T1>.Case.Assertion(tc, a.Action, a.Description)));
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
        public TestBuilderWithActionAndRVAssertions<T1> And(
            Action<T1> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new Assertion(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1> And(Expression<Action<T1>> assertion)
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
        public TestBuilderWithActionAndRVAssertions<T1> And(Action<T1> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<T1> action, string description)
            {
                Action = (a, outcome) =>
                {
                    outcome.ThrowIfException();
                    action(a);
                };
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<T1>> expression)
            {
                Action = (a, outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(a);
                };
                Description = expression.Body.ToString();
            }
#endif

            internal Assertion(string description)
            {
                Action = (a, outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<T1, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 2 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithActionAndRVAssertions<T1, T2>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction,
            Assertion assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndRVAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndRVAssertions<T1, T2> builder)
        {
            return new TestAction<T1, T2>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction<T1, T2>.Case.Assertion(tc, a.Action, a.Description)));
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
        public TestBuilderWithActionAndRVAssertions<T1, T2> And(
            Action<T1, T2> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new Assertion(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2> And(Expression<Action<T1, T2>> assertion)
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
        public TestBuilderWithActionAndRVAssertions<T1, T2> And(Action<T1, T2> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<T1, T2> action, string description)
            {
                Action = (a1, a2, outcome) =>
                {
                    outcome.ThrowIfException();
                    action(a1, a2);
                };
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<T1, T2>> expression)
            {
                Action = (a1, a2, outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(a1, a2);
                };
                Description = expression.Body.ToString();
            }
#endif

            internal Assertion(string description)
            {
                Action = (a1, a2, outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<T1, T2, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 3 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    public sealed class TestBuilderWithActionAndRVAssertions<T1, T2, T3>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction,
            Assertion assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="TestBuilderWithActionAndRVAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndRVAssertions<T1, T2, T3> builder)
        {
            return new TestAction<T1, T2, T3>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new TestAction<T1, T2, T3>.Case.Assertion(tc, a.Action, a.Description)));
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
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> And(
            Action<T1, T2, T3> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new Assertion(assertion, description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }
#else
        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3>> assertion)
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
        public TestBuilderWithActionAndRVAssertions<T1, T2, T3> And(Action<T1, T2, T3> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3> action, string description)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    outcome.ThrowIfException();
                    action(a1, a2, a3);
                };
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<T1, T2, T3>> expression)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    outcome.ThrowIfException();
                    expression.Compile()(a1, a2, a3);
                };
                Description = expression.Body.ToString();
            }
#endif

            internal Assertion(string description)
            {
                Action = (a1, a2, a3, outcome) =>
                {
                    outcome.ThrowIfException();
                };
                Description = description;
            }

            public Action<T1, T2, T3, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }
}
