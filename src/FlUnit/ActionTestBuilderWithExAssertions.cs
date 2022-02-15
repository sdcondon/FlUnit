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
    /// Builder for providing additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    public sealed class ActionTestBuilderWithExAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Action testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Action testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithExAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithExAssertions builder)
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
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions And(
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
        public ActionTestBuilderWithExAssertions And(Expression<Action<Exception>> assertion)
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
        public ActionTestBuilderWithExAssertions And(Action<Exception> assertion, string description)
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

            internal void Invoke(TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 1 "Given" clause
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithExAssertions<T1>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithExAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithExAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithExAssertions<T1> builder)
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
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1> And(
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
        public ActionTestBuilderWithExAssertions<T1> And(Expression<Action<T1, Exception>> assertion)
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
        public ActionTestBuilderWithExAssertions<T1> And(Action<T1, Exception> assertion, string description)
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

            internal void Invoke(T1 a1, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 2 "Given" clauses
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithExAssertions<T1, T2>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithExAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithExAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithExAssertions<T1, T2> builder)
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
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2> And(
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
        public ActionTestBuilderWithExAssertions<T1, T2> And(Expression<Action<T1, T2, Exception>> assertion)
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
        public ActionTestBuilderWithExAssertions<T1, T2> And(Action<T1, T2, Exception> assertion, string description)
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

            internal void Invoke(T1 a1, T2 a2, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, a2, outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 3 "Given" clauses
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithExAssertions<T1, T2, T3>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithExAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithExAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithExAssertions<T1, T2, T3> builder)
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
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2, T3> And(
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
        public ActionTestBuilderWithExAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, Exception>> assertion)
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
        public ActionTestBuilderWithExAssertions<T1, T2, T3> And(Action<T1, T2, T3, Exception> assertion, string description)
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

            internal void Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, a2, a3, outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 4 "Given" clauses
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithExAssertions<T1, T2, T3, T4>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>, Func<IEnumerable<T4>>) arrange;
        private readonly Action<T1, T2, T3, T4> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>, Func<IEnumerable<T4>>) arrange,
            Action<T1, T2, T3, T4> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithExAssertions{T1, T2, T3, T4}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithExAssertions<T1, T2, T3, T4> builder)
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
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4> And(
            Action<T1, T2, T3, T4, Exception> assertion,
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
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4> And(Expression<Action<T1, T2, T3, T4, Exception>> assertion)
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
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4> And(Action<T1, T2, T3, T4, Exception> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3, T4, Exception> assert;

            internal AssertionImpl(Action<T1, T2, T3, T4, Exception> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, Exception>> expression)
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

            internal void Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, a2, a3, a4, outcome.Exception);
            }
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 5 "Given" clauses
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithExAssertions<T1, T2, T3, T4, T5>
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>, Func<IEnumerable<T4>>, Func<IEnumerable<T5>>) arrange;
        private readonly Action<T1, T2, T3, T4, T5> testAction;
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>, Func<IEnumerable<T4>>, Func<IEnumerable<T5>>) arrange,
            Action<T1, T2, T3, T4, T5> testAction,
            AssertionImpl assertion)
        {
            this.configurationOverrides = configurationOverrides;
            this.arrange = arrange;
            this.testAction = testAction;
            assertions.Add(assertion);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ActionTestBuilderWithExAssertions{T1, T2, T3, T4, T5}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithExAssertions<T1, T2, T3, T4, T5> builder)
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
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4, T5> And(
            Action<T1, T2, T3, T4, T5, Exception> assertion,
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
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4, T5> And(Expression<Action<T1, T2, T3, T4, T5, Exception>> assertion)
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
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4, T5> And(Action<T1, T2, T3, T4, T5, Exception> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion, description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
            private readonly Action<T1, T2, T3, T4, T5, Exception> assert;

            internal AssertionImpl(Action<T1, T2, T3, T4, T5, Exception> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

#if !NET6_0
            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, T5, Exception>> expression)
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

            internal void Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                assert?.Invoke(a1, a2, a3, a4, a5, outcome.Exception);
            }
        }
    }
}
