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
    public sealed class ActionTestBuilderWithAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Action testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithAssertions builder)
        {
            return new ActionTest(
                builder.configurationOverrides,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithAssertions And(
            Action<TestActionOutcome> assertion,
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
        public ActionTestBuilderWithAssertions And(Expression<Action<TestActionOutcome>> assertion)
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
        public ActionTestBuilderWithAssertions And(Action<TestActionOutcome> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<TestActionOutcome> assert;

            internal AssertionImpl(Action<TestActionOutcome> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<TestActionOutcome>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal void Invoke(TestActionOutcome outcome) => assert(outcome);

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 1 "Given" clause
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithAssertions<T1>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ITestContext, IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, IEnumerable<T1>> arrange,
            Action<T1> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithAssertions<T1> builder)
        {
            return new ActionTest<T1>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1>.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithAssertions<T1> And(
            Action<T1, TestActionOutcome> assertion,
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
        public ActionTestBuilderWithAssertions<T1> And(Expression<Action<T1, TestActionOutcome>> assertion)
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
        public ActionTestBuilderWithAssertions<T1> And(Action<T1, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, TestActionOutcome> assert;

            internal AssertionImpl(Action<T1, TestActionOutcome> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, TestActionOutcome>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal void Invoke(T1 a1, TestActionOutcome outcome) => assert(a1, outcome);

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 2 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithAssertions<T1, T2>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>) arrange,
            Action<T1, T2> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithAssertions<T1, T2> builder)
        {
            return new ActionTest<T1, T2>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1, T2>.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithAssertions<T1, T2> And(
            Action<T1, T2, TestActionOutcome> assertion,
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
        public ActionTestBuilderWithAssertions<T1, T2> And(Expression<Action<T1, T2, TestActionOutcome>> assertion)
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
        public ActionTestBuilderWithAssertions<T1, T2> And(Action<T1, T2, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, TestActionOutcome> assert;

            internal AssertionImpl(Action<T1, T2, TestActionOutcome> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, TestActionOutcome>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal void Invoke(T1 a1, T2 a2, TestActionOutcome outcome) => assert(a1, a2, outcome);

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
    public sealed class ActionTestBuilderWithAssertions<T1, T2, T3>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithAssertions<T1, T2, T3> builder)
        {
            return new ActionTest<T1, T2, T3>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1, T2, T3>.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithAssertions<T1, T2, T3> And(
            Action<T1, T2, T3, TestActionOutcome> assertion,
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
        public ActionTestBuilderWithAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, TestActionOutcome>> assertion)
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
        public ActionTestBuilderWithAssertions<T1, T2, T3> And(Action<T1, T2, T3, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3, TestActionOutcome> assert;

            internal AssertionImpl(Action<T1, T2, T3, TestActionOutcome> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3, TestActionOutcome>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal void Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome) => assert(a1, a2, a3, outcome);

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 4 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithAssertions<T1, T2, T3, T4>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>) arrange;
        private readonly Action<T1, T2, T3, T4> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>) arrange,
            Action<T1, T2, T3, T4> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithAssertions{T1, T2, T3, T4}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithAssertions<T1, T2, T3, T4> builder)
        {
            return new ActionTest<T1, T2, T3, T4>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1, T2, T3, T4>.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4> And(
            Action<T1, T2, T3, T4, TestActionOutcome> assertion,
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
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4> And(Expression<Action<T1, T2, T3, T4, TestActionOutcome>> assertion)
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
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4> And(Action<T1, T2, T3, T4, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3, T4, TestActionOutcome> assert;

            internal AssertionImpl(Action<T1, T2, T3, T4, TestActionOutcome> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, TestActionOutcome>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal void Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome) => assert(a1, a2, a3, a4, outcome);

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a 5 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>, Func<ITestContext, IEnumerable<T5>>) arrange;
        private readonly Action<T1, T2, T3, T4, T5> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, IEnumerable<T1>>, Func<ITestContext, IEnumerable<T2>>, Func<ITestContext, IEnumerable<T3>>, Func<ITestContext, IEnumerable<T4>>, Func<ITestContext, IEnumerable<T5>>) arrange,
            Action<T1, T2, T3, T4, T5> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithAssertions{T1, T2, T3, T4, T5}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5> builder)
        {
            return new ActionTest<T1, T2, T3, T4, T5>(
                builder.configurationOverrides,
                builder.arrange,
                builder.testAction,
                tc => builder.assertions.Select(a => new ActionTest<T1, T2, T3, T4, T5>.Assertion(tc, a.Invoke, a.Description)));
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
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5> And(
            Action<T1, T2, T3, T4, T5, TestActionOutcome> assertion,
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
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5> And(Expression<Action<T1, T2, T3, T4, T5, TestActionOutcome>> assertion)
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
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5> And(Action<T1, T2, T3, T4, T5, TestActionOutcome> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3, T4, T5, TestActionOutcome> assert;

            internal AssertionImpl(Action<T1, T2, T3, T4, T5, TestActionOutcome> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, T5, TestActionOutcome>> expression)
            {
                assert = expression.Compile();
                Description = expression.Body.ToString();
            }
#endif

            internal void Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome) => assert(a1, a2, a3, a4, a5, outcome);

            public string Description { get; }
        }
    }
}
