using System;
using System.Collections.Generic;
using System.Linq;

namespace FlUnit
{
    //// TODO: T4 template me to eliminate repetition and allow for more prereqs with no effort

    /// <summary>
    /// Represents a test with no "Given" clauses and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction : Test
    {
        private readonly Action act;
        private TestActionResult invocationResult;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Test"/> class.
        /// </summary>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction(Action act, IEnumerable<TestBuilderWithActionAndAssertions.Assertion> assertions)
        {
            this.act = act;
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
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
                act();
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestAction test;
            private readonly Action<TestActionResult> action;

            public Assertion(TestAction test, Action<TestActionResult> action, string description)
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
    /// Represents a test with one "Given" clause and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction<T1> : Test
    {
        private readonly T1 prereq;
        private readonly Action<T1> act;
        private TestActionResult invocationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction{T1}"/> class.
        /// </summary>
        /// <param name="prereq"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction(T1 prereq, Action<T1> act, IEnumerable<TestBuilderWithActionAndAssertions<T1>.Assertion> assertions)
        {
            this.prereq = prereq;
            this.act = act;
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
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
                act(prereq);
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestAction<T1> test;
            private readonly Action<T1, TestActionResult> action;

            public Assertion(TestAction<T1> test, Action<T1, TestActionResult> action, string description)
            {
                this.test = test;
                this.action = action;
                this.Description = description;
            }

            public override string Description { get; }

            public override void Invoke() => action(test.prereq, test.invocationResult);
        }
    }

    /// <summary>
    /// Represents a test with two "Given" clause and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction<T1, T2> : Test
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> act;
        private TestActionResult invocationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction{T1, T2}"/> class.
        /// </summary>
        /// <param name="prereqs"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction((T1, T2) prereqs, Action<T1, T2> act, IEnumerable<TestBuilderWithActionAndAssertions<T1, T2>.Assertion> assertions)
        {
            this.prereqs = prereqs;
            this.act = act;
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
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
                act(prereqs.Item1, prereqs.Item2);
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestAction<T1, T2> test;
            private readonly Action<T1, T2, TestActionResult> action;

            public Assertion(TestAction<T1, T2> test, Action<T1, T2, TestActionResult> action, string description)
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
    /// Represents a test with three "Given" clause and a "When" clause that does not return a value.
    /// </summary>
    public sealed class TestAction<T1, T2, T3> : Test
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> act;
        private TestActionResult invocationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAction{T1, T2, T3}"/> class.
        /// </summary>
        /// <param name="prereqs"></param>
        /// <param name="act"></param>
        /// <param name="assertions"></param>
        internal TestAction((T1, T2, T3) prereqs, Action<T1, T2, T3> act, IEnumerable<TestBuilderWithActionAndAssertions<T1, T2, T3>.Assertion> assertions)
        {
            this.prereqs = prereqs;
            this.act = act;
            this.Assertions = assertions.Select(a => new Assertion(this, a.Action, a.Description));
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
                act(prereqs.Item1, prereqs.Item2, prereqs.Item3);
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        /// <summary>
        /// Named assertions that should all succeed (that is, not throw) once <see cref="Act"/> has been invoked.
        /// </summary>
        public override IEnumerable<TestAssertion> Assertions { get; }

        private class Assertion : TestAssertion
        {
            private readonly TestAction<T1, T2, T3> test;
            private readonly Action<T1, T2, T3, TestActionResult> action;

            public Assertion(TestAction<T1, T2, T3> test, Action<T1, T2, T3, TestActionResult> action, string description)
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
