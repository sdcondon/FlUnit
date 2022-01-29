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
    /// Builder for providing additional assertions for a test with a 0 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class TestBuilderWithActionAndAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Action testAction,
            Assertion assertion)
        {
            this.configurationOverrides = configurationOverrides;
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
        public TestBuilderWithActionAndAssertions And(
            Action<TestActionOutcome> assertion,
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
        public TestBuilderWithActionAndAssertions And(Expression<Action<TestActionOutcome>> assertion)
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
        public TestBuilderWithActionAndAssertions And(Action<TestActionOutcome> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<TestActionOutcome> action, string description)
            {
                Action = action;
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<TestActionOutcome>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

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
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
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
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1> builder)
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
        public TestBuilderWithActionAndAssertions<T1> And(
            Action<T1, TestActionOutcome> assertion,
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
        public TestBuilderWithActionAndAssertions<T1> And(Expression<Action<T1, TestActionOutcome>> assertion)
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
        public TestBuilderWithActionAndAssertions<T1> And(Action<T1, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<T1, TestActionOutcome> action, string description)
            {
                Action = action;
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<T1, TestActionOutcome>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

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
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
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
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1, T2> builder)
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
        public TestBuilderWithActionAndAssertions<T1, T2> And(
            Action<T1, T2, TestActionOutcome> assertion,
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
        public TestBuilderWithActionAndAssertions<T1, T2> And(Expression<Action<T1, T2, TestActionOutcome>> assertion)
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
        public TestBuilderWithActionAndAssertions<T1, T2> And(Action<T1, T2, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, TestActionOutcome> action, string description)
            {
                Action = action;
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<T1, T2, TestActionOutcome>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

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
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithActionAndAssertions(
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
        /// Implicitly converts a <see cref="TestBuilderWithActionAndAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(TestBuilderWithActionAndAssertions<T1, T2, T3> builder)
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
        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(
            Action<T1, T2, T3, TestActionOutcome> assertion,
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
        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, TestActionOutcome>> assertion)
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
        public TestBuilderWithActionAndAssertions<T1, T2, T3> And(Action<T1, T2, T3, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }
#endif

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3, TestActionOutcome> action, string description)
            {
                Action = action;
                Description = description;
            }

#if !NET6_0
            internal Assertion(Expression<Action<T1, T2, T3, TestActionOutcome>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            public Action<T1, T2, T3, TestActionOutcome> Action { get; }

            public string Description { get; }
        }
    }
}
