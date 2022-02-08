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
    public sealed class ActionTestBuilderWithRVAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Action testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions builder)
        {
            return new ActionTest(
                builder.configurationOverrides,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithRVAssertions And(
            Action assertion,
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
        public ActionTestBuilderWithRVAssertions And(Expression<Action> assertion)
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
        public ActionTestBuilderWithRVAssertions And(Action assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action assert;

            internal AssertionImpl(Action assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action> expression)
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

            internal void Invoke(TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                assert?.Invoke();
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 1 "Given" clause,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithRVAssertions<T1>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<IEnumerable<T1>> arrange,
            Action<T1> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1> builder)
        {
            return new ActionTest<T1>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithRVAssertions<T1> And(
            Action<T1> assertion,
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
        public ActionTestBuilderWithRVAssertions<T1> And(Expression<Action<T1>> assertion)
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
        public ActionTestBuilderWithRVAssertions<T1> And(Action<T1> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1> assert;

            internal AssertionImpl(Action<T1> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1>> expression)
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

            internal void Invoke(T1 a1, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                assert?.Invoke(a1);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 2 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithRVAssertions<T1, T2>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1, T2> builder)
        {
            return new ActionTest<T1, T2>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1, T2>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithRVAssertions<T1, T2> And(
            Action<T1, T2> assertion,
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
        public ActionTestBuilderWithRVAssertions<T1, T2> And(Expression<Action<T1, T2>> assertion)
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
        public ActionTestBuilderWithRVAssertions<T1, T2> And(Action<T1, T2> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2> assert;

            internal AssertionImpl(Action<T1, T2> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2>> expression)
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

            internal void Invoke(T1 a1, T2 a2, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                assert?.Invoke(a1, a2);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 3 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithRVAssertions<T1, T2, T3>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1, T2, T3> builder)
        {
            return new ActionTest<T1, T2, T3>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1, T2, T3>.Case.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> And(
            Action<T1, T2, T3> assertion,
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
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3>> assertion)
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
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> And(Action<T1, T2, T3> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3> assert;

            internal AssertionImpl(Action<T1, T2, T3> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3>> expression)
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

            internal void Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                assert?.Invoke(a1, a2, a3);
            }
        }
    }
}
