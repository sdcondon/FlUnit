using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
    /// <summary>
    /// Represents a test with 0 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction : Test
    {
        private readonly Action act;
        private readonly IEnumerable<TestBuilderWithActionAndAssertions.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction"/> class.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction(
            Action act,
            IEnumerable<TestBuilderWithActionAndAssertions.Assertion> assertions)
        {
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// A collection of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IReadOnlyCollection<ITestCase> Cases { get; protected set; }

        /// <inheritdoc />
        public override void Arrange()
        {
            Cases = new[] { new Case(act, assertions) };
        }

        private class Case : ITestCase
        {
            private readonly Action act;
            private TestActionOutcome invocationOutcome;

            internal Case(
                Action act,
                IEnumerable<TestBuilderWithActionAndAssertions.Assertion> assertions)
            {
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => string.Empty;

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act();
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<TestActionOutcome> action;

                public Assertion(
                    Case testCase,
                    Action<TestActionOutcome> action,
                    string description)
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
    /// Represents a test with 1 "Given" clause and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction<T1> : Test
    {
        private readonly Func<IEnumerable<T1>> arrange;
        private readonly Action<T1> act;
        private readonly IEnumerable<TestBuilderWithActionAndAssertions<T1>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction{T1}"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction(
            Func<IEnumerable<T1>> arrange,
            Action<T1> act,
            IEnumerable<TestBuilderWithActionAndAssertions<T1>.Assertion> assertions)
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

        /// <inheritdoc />
        public override void Arrange()
        {
            Cases = arrange().Select(p => new Case(p, act, assertions)).ToArray();
        }

        private class Case : ITestCase
        {
            private readonly Action<T1> act;
            private readonly T1 prereqs;
            private TestActionOutcome invocationOutcome;

            internal Case(
                T1 prereqs,
                Action<T1> act,
                IEnumerable<TestBuilderWithActionAndAssertions<T1>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => prereqs.ToString();

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act(prereqs);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, TestActionOutcome> action;

                public Assertion(
                    Case testCase,
                    Action<T1, TestActionOutcome> action,
                    string description)
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
    /// Represents a test with 2 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction<T1, T2> : Test
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange;
        private readonly Action<T1, T2> act;
        private readonly IEnumerable<TestBuilderWithActionAndAssertions<T1, T2>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction{T1, T2}"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>) arrange,
            Action<T1, T2> act,
            IEnumerable<TestBuilderWithActionAndAssertions<T1, T2>.Assertion> assertions)
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

        /// <inheritdoc />
        public override void Arrange()
        {
            Cases = (
                from p1 in arrange.Item1()
                from p2 in arrange.Item2()
                select new Case((p1, p2), act, assertions)).ToArray();
        }

        private class Case : ITestCase
        {
            private readonly Action<T1, T2> act;
            private readonly (T1, T2) prereqs;
            private TestActionOutcome invocationOutcome;

            internal Case(
                (T1, T2) prereqs,
                Action<T1, T2> act,
                IEnumerable<TestBuilderWithActionAndAssertions<T1, T2>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => prereqs.ToString();

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act(prereqs.Item1, prereqs.Item2);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, TestActionOutcome> action;

                public Assertion(
                    Case testCase,
                    Action<T1, T2, TestActionOutcome> action,
                    string description)
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
    /// Represents a test with 3 "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction<T1, T2, T3> : Test
    {
        private readonly (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange;
        private readonly Action<T1, T2, T3> act;
        private readonly IEnumerable<TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction(
            (Func<IEnumerable<T1>>, Func<IEnumerable<T2>>, Func<IEnumerable<T3>>) arrange,
            Action<T1, T2, T3> act,
            IEnumerable<TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion> assertions)
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

        /// <inheritdoc />
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
            private readonly Action<T1, T2, T3> act;
            private readonly (T1, T2, T3) prereqs;
            private TestActionOutcome invocationOutcome;

            internal Case(
                (T1, T2, T3) prereqs,
                Action<T1, T2, T3> act,
                IEnumerable<TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description)).ToArray();
            }

            public string Description => prereqs.ToString();

            /// <inheritdoc />
            public IReadOnlyCollection<ITestAssertion> Assertions { get; }

            /// <inheritdoc />
            public void Act()
            {
                if (invocationOutcome != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    act(prereqs.Item1, prereqs.Item2, prereqs.Item3);
                    invocationOutcome = new TestActionOutcome();
                }
                catch (Exception e)
                {
                    invocationOutcome = new TestActionOutcome(e);
                }
            }

            private class Assertion : ITestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, T3, TestActionOutcome> action;

                public Assertion(
                    Case testCase,
                    Action<T1, T2, T3, TestActionOutcome> action,
                    string description)
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