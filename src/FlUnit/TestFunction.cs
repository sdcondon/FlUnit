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
        private TestFunctionResult<TResult> invocationResult;

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
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
        }

        public override void Arrange()
        {
        }

        /// <summary>
        /// Invokes the test action.
        /// </summary>
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

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestFunction<TResult> test;
            private readonly Action<TestFunctionResult<TResult>> action;

            public Assertion(TestFunction<TResult> test, Action<TestFunctionResult<TResult>> action, string description)
            {
                this.test = test;
                this.action = action;
                this.Description = description;
            }

            public override string Description { get; }

            public override void Invoke() => action(test.invocationResult);
        }
    }

    /// <summary>
    /// Represents a test with 1 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, TResult> : Test
    {
        private readonly Func<T1> arrange;
        private readonly Func<T1, TResult> act;
        private T1 prereqs;
        private TestFunctionResult<TResult> invocationResult;

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
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
        }

        public override void Arrange()
        {
            prereqs = arrange();
        }

        /// <summary>
        /// Invokes the test action.
        /// </summary>
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

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestFunction<T1, TResult> test;
            private readonly Action<T1, TestFunctionResult<TResult>> action;

            public Assertion(TestFunction<T1, TResult> test, Action<T1, TestFunctionResult<TResult>> action, string description)
            {
                this.test = test;
                this.action = action;
                this.Description = description;
            }

            public override string Description { get; }

            public override void Invoke() => action(test.prereqs, test.invocationResult);
        }
    }

    /// <summary>
    /// Represents a test with 2 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, TResult> : Test
    {
        private readonly (Func<T1>, Func<T2>) arrange;
        private readonly Func<T1, T2, TResult> act;
        private (T1, T2) prereqs;
        private TestFunctionResult<TResult> invocationResult;

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
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
        }

        public override void Arrange()
        {
            (prereqs.Item1, prereqs.Item2) = (arrange.Item1(), arrange.Item2());
        }

        /// <summary>
        /// Invokes the test action.
        /// </summary>
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

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestFunction<T1, T2, TResult> test;
            private readonly Action<T1, T2, TestFunctionResult<TResult>> action;

            public Assertion(TestFunction<T1, T2, TResult> test, Action<T1, T2, TestFunctionResult<TResult>> action, string description)
            {
                this.test = test;
                this.action = action;
                this.Description = description;
            }

            public override string Description { get; }

            public override void Invoke() => action(test.prereqs.Item1, test.prereqs.Item2, test.invocationResult);
        }
    }

    /// <summary>
    /// Represents a test with 3 "Given" clauses and a "When" clause that returns a value.
    /// </summary>
    public sealed class TestFunction<T1, T2, T3, TResult> : Test
    {
        private readonly (Func<T1>, Func<T2>, Func<T3>) arrange;
        private readonly Func<T1, T2, T3, TResult> act;
        private (T1, T2, T3) prereqs;
        private TestFunctionResult<TResult> invocationResult;

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
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
        }

        public override void Arrange()
        {
            (prereqs.Item1, prereqs.Item2, prereqs.Item3) = (arrange.Item1(), arrange.Item2(), arrange.Item3());
        }

        /// <summary>
        /// Invokes the test action.
        /// </summary>
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

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestFunction<T1, T2, T3, TResult> test;
            private readonly Action<T1, T2, T3, TestFunctionResult<TResult>> action;

            public Assertion(TestFunction<T1, T2, T3, TResult> test, Action<T1, T2, T3, TestFunctionResult<TResult>> action, string description)
            {
                this.test = test;
                this.action = action;
                this.Description = description;
            }

            public override string Description { get; }

            public override void Invoke() => action(test.prereqs.Item1, test.prereqs.Item2, test.prereqs.Item3, test.invocationResult);
        }
    }
}