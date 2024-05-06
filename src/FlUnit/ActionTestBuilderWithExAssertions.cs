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
    /// Builder for providing additional assertions for a test with 0 "Given" clauses
    /// and for which the "When" clause does not return a value, and is in fact expected to throw an exception.
    /// </summary>
    public sealed class ActionTestBuilderWithExAssertions
    {
        private readonly IEnumerable<Action<ITestConfiguration>> configurationOverrides;
        private readonly Func<ValueTask> testAction;
        private readonly List<AssertionImpl> assertions = new();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ValueTask> testAction,
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

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions AndAsync(
            Func<Exception, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<Exception, ValueTask> assert;

            internal AssertionImpl(Func<Exception, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                return assert?.Invoke(outcome.Exception) ?? ValueTask.CompletedTask;
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
        private readonly Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange;
        private readonly Func<T1, ValueTask> testAction;
        private readonly List<AssertionImpl> assertions = new();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            Func<ITestContext, ValueTask<IEnumerable<T1>>> arrange,
            Func<T1, ValueTask> testAction,
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

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1> AndAsync(
            Func<T1, Exception, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, Exception, ValueTask> assert;

            internal AssertionImpl(Func<T1, Exception, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                return assert?.Invoke(a1, outcome.Exception) ?? ValueTask.CompletedTask;
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
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange;
        private readonly Func<T1, T2, ValueTask> testAction;
        private readonly List<AssertionImpl> assertions = new();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>) arrange,
            Func<T1, T2, ValueTask> testAction,
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

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2> AndAsync(
            Func<T1, T2, Exception, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, Exception, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, Exception, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                return assert?.Invoke(a1, a2, outcome.Exception) ?? ValueTask.CompletedTask;
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
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange;
        private readonly Func<T1, T2, T3, ValueTask> testAction;
        private readonly List<AssertionImpl> assertions = new();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>) arrange,
            Func<T1, T2, T3, ValueTask> testAction,
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

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2, T3> AndAsync(
            Func<T1, T2, T3, Exception, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, T3, Exception, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, Exception, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                return assert?.Invoke(a1, a2, a3, outcome.Exception) ?? ValueTask.CompletedTask;
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
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange;
        private readonly Func<T1, T2, T3, T4, ValueTask> testAction;
        private readonly List<AssertionImpl> assertions = new();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>) arrange,
            Func<T1, T2, T3, T4, ValueTask> testAction,
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

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4> AndAsync(
            Func<T1, T2, T3, T4, Exception, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, T3, T4, Exception, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, Exception, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                return assert?.Invoke(a1, a2, a3, a4, outcome.Exception) ?? ValueTask.CompletedTask;
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
        private readonly (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange;
        private readonly Func<T1, T2, T3, T4, T5, ValueTask> testAction;
        private readonly List<AssertionImpl> assertions = new();

        internal ActionTestBuilderWithExAssertions(
            IEnumerable<Action<ITestConfiguration>> configurationOverrides,
            (Func<ITestContext, ValueTask<IEnumerable<T1>>>, Func<ITestContext, ValueTask<IEnumerable<T2>>>, Func<ITestContext, ValueTask<IEnumerable<T3>>>, Func<ITestContext, ValueTask<IEnumerable<T4>>>, Func<ITestContext, ValueTask<IEnumerable<T5>>>) arrange,
            Func<T1, T2, T3, T4, T5, ValueTask> testAction,
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

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
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
            assertions.Add(new AssertionImpl(assertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="asyncAssertion">The assertion. It should have one parameter for each "Given" clause (if any), and a final one for the thrown exception.</param>
        /// <param name="description">The description of the assertion. Optional</param>
        /// <param name="assertionExpression">
        /// Automatically populated by the compiler - takes the value of the argument expression passed to the assertion parameter.
        /// Used as the description of the assertion if no description is provided - after a little processing (namely, lambda expressions are trimmed so that only their body remains).
        /// </param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public ActionTestBuilderWithExAssertions<T1, T2, T3, T4, T5> AndAsync(
            Func<T1, T2, T3, T4, T5, Exception, Task> asyncAssertion,
            string description = null,
            [CallerArgumentExpression("asyncAssertion")] string assertionExpression = null)
        {
            assertions.Add(new AssertionImpl(asyncAssertion.ToAsyncWrapper(), description ?? AssertionExpressionHelpers.ToAssertionDescription(assertionExpression)));
            return this;
        }

        internal class AssertionImpl
        {
            private readonly Func<T1, T2, T3, T4, T5, Exception, ValueTask> assert;

            internal AssertionImpl(Func<T1, T2, T3, T4, T5, Exception, ValueTask> assert, string description)
            {
                this.assert = assert;
                Description = description;
            }

            internal AssertionImpl(string description)
            {
                Description = description;
            }

            public string Description { get; }

            internal ValueTask Invoke(T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, TestActionOutcome outcome)
            {
                outcome.ThrowIfNoException();
                return assert?.Invoke(a1, a2, a3, a4, a5, outcome.Exception) ?? ValueTask.CompletedTask;
            }
        }
    }
}
