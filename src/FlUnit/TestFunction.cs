using System;
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
        /// An enumerable of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IEnumerable<TestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = new[] { new Case(act, assertions) };
        }

        private class Case : TestCase
        {
            private readonly Func<TResult> act;
            private TestFunctionResult<TResult> invocationResult;

            internal Case(
                Func<TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<TResult>.Assertion> assertions)
            {
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
            }

            public override string Description => throw new NotImplementedException("Not yet implemented");

            public override void Act()
            {
                if (invocationResult != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationResult = new TestFunctionResult<TResult>(act());
                }
                catch (Exception e)
                {
                    invocationResult = new TestFunctionResult<TResult>(e);
                }
            }

            public override IEnumerable<TestAssertion> Assertions { get; }

            private class Assertion : TestAssertion
            {
                private readonly Case testCase;
                private readonly Action<TestFunctionResult<TResult>> action;

                public Assertion(Case testCase, Action<TestFunctionResult<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public override string Description { get; }

                public override void Invoke() => action(testCase.invocationResult);
            }
        }
    }

    /// <summary>
    /// Represents a test with 1 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, TResult> : Test
    {
        private readonly Func<T1> arrange;
        private readonly Func<T1, TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            Func<T1> arrange,
            Func<T1, TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion> assertions)
        {
            this.arrange = arrange;
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// An enumerable of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IEnumerable<TestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = new[] { new Case(arrange(), act, assertions) };
        }

        private class Case : TestCase
        {
            private readonly Func<T1, TResult> act;
            private readonly T1 prereqs;
            private TestFunctionResult<TResult> invocationResult;

            internal Case(
                T1 prereqs,
                Func<T1, TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<T1, TResult>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
            }

            public override string Description => throw new NotImplementedException("Not yet implemented");

            public override void Act()
            {
                if (invocationResult != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationResult = new TestFunctionResult<TResult>(act(prereqs));
                }
                catch (Exception e)
                {
                    invocationResult = new TestFunctionResult<TResult>(e);
                }
            }

            public override IEnumerable<TestAssertion> Assertions { get; }

            private class Assertion : TestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, TestFunctionResult<TResult>> action;

                public Assertion(Case testCase, Action<T1, TestFunctionResult<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public override string Description { get; }

                public override void Invoke() => action(testCase.prereqs, testCase.invocationResult);
            }
        }
    }

    /// <summary>
    /// Represents a test with 2 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, TResult> : Test
    {
        private readonly (Func<T1>, Func<T2>) arrange;
        private readonly Func<T1, T2, TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            (Func<T1>, Func<T2>) arrange,
            Func<T1, T2, TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion> assertions)
        {
            this.arrange = arrange;
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// An enumerable of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IEnumerable<TestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = new[] { new Case((arrange.Item1(), arrange.Item2()), act, assertions) };
        }

        private class Case : TestCase
        {
            private readonly Func<T1, T2, TResult> act;
            private readonly (T1, T2) prereqs;
            private TestFunctionResult<TResult> invocationResult;

            internal Case(
                (T1, T2) prereqs,
                Func<T1, T2, TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, TResult>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
            }

            public override string Description => throw new NotImplementedException("Not yet implemented");

            public override void Act()
            {
                if (invocationResult != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationResult = new TestFunctionResult<TResult>(act(prereqs.Item1, prereqs.Item2));
                }
                catch (Exception e)
                {
                    invocationResult = new TestFunctionResult<TResult>(e);
                }
            }

            public override IEnumerable<TestAssertion> Assertions { get; }

            private class Assertion : TestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, TestFunctionResult<TResult>> action;

                public Assertion(Case testCase, Action<T1, T2, TestFunctionResult<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public override string Description { get; }

                public override void Invoke() => action(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.invocationResult);
            }
        }
    }

    /// <summary>
    /// Represents a test with 3 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, T3, TResult> : Test
    {
        private readonly (Func<T1>, Func<T2>, Func<T3>) arrange;
        private readonly Func<T1, T2, T3, TResult> act;
        private readonly IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion> assertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="arrange"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestFunction(
            (Func<T1>, Func<T2>, Func<T3>) arrange,
            Func<T1, T2, T3, TResult> act,
            IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion> assertions)
        {
            this.arrange = arrange;
            this.act = act;
            this.assertions = assertions;
        }

        /// <summary>
        /// An enumerable of test cases that should be populated once <see cref="Arrange"/> is called.
        /// </summary>
        // TODO: Better errors: throw new InvalidOperationException("Test not yet arranged") on premature access rather than returning null.
        public override IEnumerable<TestCase> Cases { get; protected set; }

        /// <summary>
        /// Arranges the test.
        /// </summary>
        public override void Arrange()
        {
            Cases = new[] { new Case((arrange.Item1(), arrange.Item2(), arrange.Item3()), act, assertions) };
        }

        private class Case : TestCase
        {
            private readonly Func<T1, T2, T3, TResult> act;
            private readonly (T1, T2, T3) prereqs;
            private TestFunctionResult<TResult> invocationResult;

            internal Case(
                (T1, T2, T3) prereqs,
                Func<T1, T2, T3, TResult> act,
                IEnumerable<TestBuilderWithFunctionAndAssertions<T1, T2, T3, TResult>.Assertion> assertions)
            {
                this.prereqs = prereqs;
                this.act = act;
                this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
            }

            public override string Description => throw new NotImplementedException("Not yet implemented");

            public override void Act()
            {
                if (invocationResult != null)
                {
                    throw new InvalidOperationException("Test action already invoked");
                }

                try
                {
                    invocationResult = new TestFunctionResult<TResult>(act(prereqs.Item1, prereqs.Item2, prereqs.Item3));
                }
                catch (Exception e)
                {
                    invocationResult = new TestFunctionResult<TResult>(e);
                }
            }

            public override IEnumerable<TestAssertion> Assertions { get; }

            private class Assertion : TestAssertion
            {
                private readonly Case testCase;
                private readonly Action<T1, T2, T3, TestFunctionResult<TResult>> action;

                public Assertion(Case testCase, Action<T1, T2, T3, TestFunctionResult<TResult>> action, string description)
                {
                    this.testCase = testCase;
                    this.action = action;
                    this.Description = description;
                }

                public override string Description { get; }

                public override void Invoke() => action(testCase.prereqs.Item1, testCase.prereqs.Item2, testCase.prereqs.Item3, testCase.invocationResult);
            }
        }
    }
}