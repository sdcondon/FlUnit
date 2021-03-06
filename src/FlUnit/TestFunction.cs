﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
    /// <summary>
    /// Represents a test with 0 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<TResult> : Test
    {
        private readonly Func<TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            Func<TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<TResult>.Assertion> assertions)
        {
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IReadOnlyCollection<ITestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = new[] { new Case(act, assertions) };
        }

        private class Case : ITestCase
        {
            private readonly Func<TResult> act;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                Func<TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<TResult>.Assertion> assertions)
            {
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => string.Empty;

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act());
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.invocationOutcome);
            }
        }
    }

    /// <summary>
    /// Represents a test with 1 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, TResult> : Test
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Func<T1, TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            Func<IEnumerable<T1>> arrange,
            Func<T1, TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion> assertions)
        {
            this.arrange = arrange;
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IReadOnlyCollection<ITestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = arrange().Select(p => new Case(p, act, assertions)).ToArray();
        }

        private class Case : ITestCase
        {
            private readonly Func<T1, TResult> act;
            private readonly T1 prereqs;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                T1 prereqs,
                Func<T1, TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => prereqs.ToString();

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act(prereqs));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<T1, TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.prereqs, testCase.invocationOutcome);
            }
        }
    }

    /// <summary>
    /// Represents a test with 2 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, TResult> : Test
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Func<T1, T2, TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Func<T1, T2, TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion> assertions)
        {
            this.arrange = arrange;
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IReadOnlyCollection<ITestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = (
                from p1 in arrange.Item1()
                from p2 in arrange.Item2()
                select new Case((p1, p2), act, assertions)).ToArray();
        }

        private class Case : ITestCase
        {
            private readonly Func<T1, T2, TResult> act;
            private readonly (T1, T2) prereqs;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                (T1, T2) prereqs,
                Func<T1, T2, TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => prereqs.ToString();

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act(prereqs.Item1, prereqs.Item2));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<T1, T2, TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.invocationOutcome);
            }
        }
    }

    /// <summary>
    /// Represents a test with 3 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, T3, TResult> : Test
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Func<T1, T2, T3, TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Func<T1, T2, T3, TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion> assertions)
        {
            this.arrange = arrange;
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IReadOnlyCollection<ITestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = (
                from p1 in arrange.Item1()
                from p2 in arrange.Item2()
                from p3 in arrange.Item3()
                select new Case((p1, p2, p3), act, assertions)).ToArray();
        }

        private class Case : ITestCase
        {
            private readonly Func<T1, T2, T3, TResult> act;
            private readonly (T1, T2, T3) prereqs;
            private TestFunctionOutcome<TResult> invocationOutcome;

            internal Case(
                (T1, T2, T3) prereqs,
                Func<T1, T2, T3, TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => prereqs.ToString();

            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(act(prereqs.Item1, prereqs.Item2, prereqs.Item3));
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestFunctionOutcome<TResult>(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, T3, TestFunctionOutcome<TResult>> action;

                public Assertion(Case testCase, Action<T1, T2, T3, TestFunctionOutcome<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public string Description { get; }

                public void Invoke() => action(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.invocationOutcome);
            }
        }
    }
}