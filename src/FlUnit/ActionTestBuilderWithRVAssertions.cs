using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
#if NET6_0_OR_GREATER
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing additional assertions for a test with 0 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    public sealed class ActionTestBuilderWithRVAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
#if NET6_0_OR_GREATER
        private readonly Func<ValueTask> testAction;
#else
        private readonly Func<Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions builder)
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions AndAsync(
            Func<Task> asyncAssertion,
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions And(Expression<Action> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions And(Action assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions AndAsync(Func<Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<ValueTask> assert;

            internal AssertionImpl(Func<ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<Task> assert;

            internal AssertionImpl(Func<Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke() ?? ValueTask.CompletedTask;
            }
#else
            internal Task Invoke(TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke() ?? Task.CompletedTask;
            }
#endif
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
#if NET6_0_OR_GREATER
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask> testAction;
#else
        private readonly Func<ITestContext, Task<IEnumerable<T1>>> arrange;
        private readonly Func<T1, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1> builder)
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1> AndAsync(
            Func<T1, Task> asyncAssertion,
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1> And(Expression<Action<T1>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1> And(Action<T1> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1> AndAsync(Func<T1, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, ValueTask> assert;

            internal AssertionImpl(Func<T1, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, Task> assert;

            internal AssertionImpl(Func<T1, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1) ?? ValueTask.CompletedTask;
            }
#else
            internal Task Invoke(T1 a1, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1) ?? Task.CompletedTask;
            }
#endif
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
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask> testAction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1, T2}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1, T2> builder)
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2> AndAsync(
            Func<T1, T2, Task> asyncAssertion,
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2> And(Expression<Action<T1, T2>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2> And(Action<T1, T2> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2> AndAsync(Func<T1, T2, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, Task> assert;

            internal AssertionImpl(Func<T1, T2, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2) ?? ValueTask.CompletedTask;
            }
#else
            internal Task Invoke(T1 a1, T2 a2, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2) ?? Task.CompletedTask;
            }
#endif
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
#if NET6_0_OR_GREATER
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask> testAction;
#else
        private readonly (Func<ITestContext, Task<IEnumerable<T1>>>, Func<ITestContext, Task<IEnumerable<T2>>>, Func<ITestContext, Task<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, Task> testAction;
#endif
        private readonly List<AssertionImpl> assertions = new List<AssertionImpl>();

        internal ActionTestBuilderWithRVAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1, T2, T3}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1, T2, T3> builder)
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> AndAsync(
            Func<T1, T2, T3, Task> asyncAssertion,
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> And(Action<T1, T2, T3> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3> AndAsync(Func<T1, T2, T3, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3) ?? ValueTask.CompletedTask;
            }
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3) ?? Task.CompletedTask;
            }
#endif
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 4 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithRVAssertions<T1, T2, T3, T4>
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

        internal ActionTestBuilderWithRVAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1, T2, T3, T4}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1, T2, T3, T4> builder)
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4> And(
            Action<T1, T2, T3, T4> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4> AndAsync(
            Func<T1, T2, T3, T4, Task> asyncAssertion,
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4> And(Expression<Action<T1, T2, T3, T4>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4> And(Action<T1, T2, T3, T4> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4> AndAsync(Func<T1, T2, T3, T4, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, T4, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, T4, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, T4>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, a4) ?? ValueTask.CompletedTask;
            }
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, a4) ?? Task.CompletedTask;
            }
#endif
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with 5 "Given" clauses,
    /// for which the "When" clause does not return a value but is expected to successfully return.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="T4">The type of the 4th "Given" clause of the test.</typeparam>
    /// <typeparam name="T5">The type of the 5th "Given" clause of the test.</typeparam>
    public sealed class ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5>
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

        internal ActionTestBuilderWithRVAssertions(
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
        /// Implicitly converts a <see cref="ActionTestBuilderWithRVAssertions{T1, T2, T3, T4, T5}"/> to a <see cref="Test"/> (by building it).
        /// </summary>
        /// <param name="builder">The builder to convert.</param>
        public static implicit operator Test(ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5> builder)
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5> And(
            Action<T1, T2, T3, T4, T5> assertion,
            string description = null,
            [CallerArgumentExpression("assertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion. Optional.</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5> AndAsync(
            Func<T1, T2, T3, T4, T5, Task> asyncAssertion,
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
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5> And(Expression<Action<T1, T2, T3, T4, T5>> assertion)
        {
            assertions.Add(new AssertionImpl(assertion));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5> And(Action<T1, T2, T3, T4, T5> assertion, string description)
        {
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any).</param>
        /// <param name="description">The description of the assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithRVAssertions<T1, T2, T3, T4, T5> AndAsync(Func<T1, T2, T3, T4, T5, Task> asyncAssertion, string description)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description));
            return this;
        }
#endif

        internal class AssertionImpl
        {
#if NET6_0_OR_GREATER
            private readonly Func<T1, T2, T3, T4, T5, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }
#else
            private readonly Func<T1, T2, T3, T4, T5, Task> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, Task> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(Expression<Action<T1, T2, T3, T4, T5>> expression)
            {
                assert = expression.Compile().ToAsyncWrapper();
                Description = expression.Body.ToString();
            }
#endif

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

#if NET6_0_OR_GREATER
            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, a4, a5) ?? ValueTask.CompletedTask;
            }
#else
            internal Task Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome)
            {
                outcome.ThrowIfException();
                return assert?.Invoke(a1, a2, a3, a4, a5) ?? Task.CompletedTask;
            }
#endif
        }
    }
}
