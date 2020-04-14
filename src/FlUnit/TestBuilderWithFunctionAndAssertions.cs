﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlUnit
{
    /// <summary>
    /// Builder for providing the additional assertions for a test with no "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndAssertions<TResult>
    {
        private readonly Func<TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndAssertions(Func<TResult> testFunction, Assertion assertion)
        {
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithFunctionAndAssertions<TResult> builder)
        {
            return new TestFunction<TResult>(builder.testFunction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<TResult> And(Expression<Action<TestFunctionResult<TResult>>> assertion)
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
        public TestBuilderWithFunctionAndAssertions<TResult> And(Action<TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<TestFunctionResult<TResult>> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<TestFunctionResult<TResult>>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<TestFunctionResult<TResult>> Action { get; }

            public string Description { get; }
        }
    }

    /// <summary>
    /// Builder for providing the additional assertions for a test with 1 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndAssertions<T1, TResult>
    {
        private readonly Func<T1> arrange;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndAssertions(Func<T1> arrange, Func<T1, TResult> testFunction, Assertion assertion)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithFunctionAndAssertions<T1, TResult> builder)
        {
            return new TestFunction<T1, TResult>(builder.arrange, builder.testFunction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, TResult> And(Expression<Action<T1, TestFunctionResult<TResult>>> assertion)
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
        public TestBuilderWithFunctionAndAssertions<T1, TResult> And(Action<T1, TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, TestFunctionResult<TResult>> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<T1, TestFunctionResult<TResult>>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<T1, TestFunctionResult<TResult>> Action { get; }

            public string Description { get; }
        }
    }
    /// <summary>
    /// Builder for providing the additional assertions for a test with 2 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndAssertions<T1, T2, TResult>
    {
        private readonly (Func<T1>, Func<T2>) arrange;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndAssertions((Func<T1>, Func<T2>) arrange, Func<T1, T2, TResult> testFunction, Assertion assertion)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithFunctionAndAssertions<T1, T2, TResult> builder)
        {
            return new TestFunction<T1, T2, TResult>(builder.arrange, builder.testFunction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> And(Expression<Action<T1, T2, TestFunctionResult<TResult>>> assertion)
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
        public TestBuilderWithFunctionAndAssertions<T1, T2, TResult> And(Action<T1, T2, TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, TestFunctionResult<TResult>> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, TestFunctionResult<TResult>>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, TestFunctionResult<TResult>> Action { get; }

            public string Description { get; }
        }
    }
    /// <summary>
    /// Builder for providing the additional assertions for a test with 3 "Given" clauses
    /// and for which the "When" clause returns a value.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st "Given" clause of the test.</typeparam>
    /// <typeparam name="T2">The type of the 2nd "Given" clause of the test.</typeparam>
    /// <typeparam name="T3">The type of the 3rd "Given" clause of the test.</typeparam>
    /// <typeparam name="TResult">The return type of the "When" clause of the test.</typeparam>
    public sealed class TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>
    {
        private readonly (Func<T1>, Func<T2>, Func<T3>) arrange;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<Assertion> assertions = new List<Assertion>();

        internal TestBuilderWithFunctionAndAssertions((Func<T1>, Func<T2>, Func<T3>) arrange, Func<T1, T2, T3, TResult> testFunction, Assertion assertion)
        {
            this.arrange = arrange;
            this.testFunction = testFunction;
            assertions.Add(assertion);
        }

        public static implicit operator Test(TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> builder)
        {
            return new TestFunction<T1, T2, T3, TResult>(builder.arrange, builder.testFunction, builder.assertions);
        }

        /// <summary>
        /// Adds an additional assertion for the test.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <returns>A builder for providing additional assertions for the test.</returns>
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> And(Expression<Action<T1, T2, T3, TestFunctionResult<TResult>>> assertion)
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
        public TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult> And(Action<T1, T2, T3, TestFunctionResult<TResult>> assertion, string description)
        {
            assertions.Add(new Assertion(assertion, description));
            return this;
        }

        internal class Assertion
        {
            internal Assertion(Action<T1, T2, T3, TestFunctionResult<TResult>> action, string description)
            {
                Action = action;
                Description = description;
            }

            internal Assertion(Expression<Action<T1, T2, T3, TestFunctionResult<TResult>>> expression)
            {
                Action = expression.Compile();
                Description = expression.Body.ToString();
            }

            public Action<T1, T2, T3, TestFunctionResult<TResult>> Action { get; }

            public string Description { get; }
        }
    }
}