using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif
using System.Threading.Tasks;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing additional assertions for a test with a 0 "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public sealed class ActionTestBuilderWithAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly Func<ValueTask> testAction;
#else
        private readonly Func<Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            Func<ValueTask> testAction,
#else
            Func<Task> testAction,
#endif
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

#if NET6_0_OR_GREATER
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions AndAsync(
            Func<TestActionOutcome, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions AndAsync(Func<TestActionOutcome, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<TestActionOutcome, ValueTask> assert;

            internal AssertionImpl(Func<TestActionOutcome, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<TestActionOutcome, Task> assert;

            internal AssertionImpl(Func<TestActionOutcome, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<TestActionOutcome>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(TestActionOutcome outcome) => assert(outcome);
#else
            internal Task Invoke(TestActionOutcome outcome) => assert(outcome);
#endif
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
#if NET6_0_OR_GREATER
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask> testAction;
#else
        private readonly Func<ITestContext, Task<IEnumerable<T1>>> arrange;
        private readonly Func<T1, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange,
            Func<T1, ValueTask> testAction,
#else
            Func<ITestContext, Task<IEnumerable<T1>>> arrange,
            Func<T1, Task> testAction,
#endif
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

#if NET6_0_OR_GREATER
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1> AndAsync(
            Func<T1, TestActionOutcome, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1> AndAsync(Func<T1, TestActionOutcome, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, TestActionOutcome, ValueTask> assert;

            internal AssertionImpl(Func<T1, TestActionOutcome, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, TestActionOutcome, Task> assert;

            internal AssertionImpl(Func<T1, TestActionOutcome, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, TestActionOutcome>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, TestActionOutcome outcome) => assert(a1, outcome);
#else
            internal Task Invoke(T1 a1, TestActionOutcome outcome) => assert(a1, outcome);
#endif
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
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask> testAction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange,
            Func<T1, T2, ValueTask> testAction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>) arrange,
            Func<T1, T2, Task> testAction,
#endif
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

#if NET6_0_OR_GREATER
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2> AndAsync(
            Func<T1, T2, TestActionOutcome, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2> AndAsync(Func<T1, T2, TestActionOutcome, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, TestActionOutcome, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, TestActionOutcome, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, TestActionOutcome, Task> assert;

            internal AssertionImpl(Func<T1, T2, TestActionOutcome, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, TestActionOutcome>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, TestActionOutcome outcome) => assert(a1, a2, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, TestActionOutcome outcome) => assert(a1, a2, outcome);
#endif
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
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask> testAction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, ValueTask> testAction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, Task> testAction,
#endif
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

#if NET6_0_OR_GREATER
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2, T3> AndAsync(
            Func<T1, T2, T3, TestActionOutcome, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2, T3> AndAsync(Func<T1, T2, T3, TestActionOutcome, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, TestActionOutcome, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, TestActionOutcome, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, TestActionOutcome, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, TestActionOutcome, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, TestActionOutcome>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome) => assert(a1, a2, a3, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome) => assert(a1, a2, a3, outcome);
#endif
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
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, ValueTask> testAction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, ValueTask> testAction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, Task> testAction,
#endif
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

#if NET6_0_OR_GREATER
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4> AndAsync(
            Func<T1, T2, T3, T4, TestActionOutcome, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4> AndAsync(Func<T1, T2, T3, T4, TestActionOutcome, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, T4, TestActionOutcome, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, TestActionOutcome, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, T4, TestActionOutcome, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, TestActionOutcome, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, TestActionOutcome>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome) => assert(a1, a2, a3, a4, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome) => assert(a1, a2, a3, a4, outcome);
#endif
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
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, ValueTask> testAction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>, Func<ITestContext, Task<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
#if NET6_0_OR_GREATER
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, ValueTask> testAction,
#else
            (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>, Func<ITestContext, Task<IEnumerable<T4>>>, Func<ITestContext, Task<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, Task> testAction,
#endif
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

#if NET6_0_OR_GREATER
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5> AndAsync(
            Func<T1, T2, T3, T4, T5, TestActionOutcome, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion.</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithAssertions<T1, T2, T3, T4, T5> AndAsync(Func<T1, T2, T3, T4, T5, TestActionOutcome, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, T4, T5, TestActionOutcome, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, TestActionOutcome, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, T4, T5, TestActionOutcome, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, TestActionOutcome, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, T5, TestActionOutcome>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome) => assert(a1, a2, a3, a4, a5, outcome);
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome) => assert(a1, a2, a3, a4, a5, outcome);
#endif
        }
    }
}
