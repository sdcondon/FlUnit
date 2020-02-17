using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace FlUnit
{
    //// TODO: T4 template me!!
    //// TODO: Probably want to separate the builder from the ITest implementation so that Act() and Assertions
    //// isn't mixed in with the builder stuff. Doing so will likely require sorting out the prereq situation, though..

    /// <summary>
    /// Builder for providing additional assertions for a test with no "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestActionAndAssertions : ITest
    {
        private readonly Action testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestActionResult invocationResult;

        internal TestActionAndAssertions(Action testAction, Expression<Action<TestActionResult>> assertion)
            : this(testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions(Action testAction, Action<TestActionResult> assertion, string assertionDescription)
        {
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestActionAndAssertions And(Expression<Action<TestActionResult>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestActionAndAssertions And(Action<TestActionResult> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                testAction();
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        private void AddAssertion(Action<TestActionResult> assertion, string description)
        {
            // TODO: Try to avoid lambda here, too
            assertions.Add(new TestAssertion(() => assertion(invocationResult), description));
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a single "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestActionAndAssertions<T1> : ITest
    {
        private readonly T1 prereq;
        private readonly Action<T1> testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestActionResult invocationResult;

        internal TestActionAndAssertions(T1 prereq, Action<T1> testAction, Expression<Action<T1, TestActionResult>> assertion)
            : this(prereq, testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions(T1 prereq, Action<T1> testAction, Action<T1, TestActionResult> assertion, string assertionDescription)
        {
            this.prereq = prereq;
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestActionAndAssertions<T1> And(Expression<Action<T1, TestActionResult>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestActionAndAssertions<T1> And(Action<T1, TestActionResult> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                testAction(prereq);
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        private void AddAssertion(Action<T1, TestActionResult> assertion, string description)
        {
            // TODO: Try to avoid lambda here, too
            assertions.Add(new TestAssertion(() => assertion(prereq, invocationResult), description));
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a two "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestActionAndAssertions<T1, T2> : ITest
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestActionResult invocationResult;

        internal TestActionAndAssertions((T1, T2) prereqs, Action<T1, T2> testAction, Expression<Action<T1, T2, TestActionResult>> assertion)
            : this(prereqs, testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions((T1, T2) prereqs, Action<T1, T2> testAction, Action<T1, T2, TestActionResult> assertion, string assertionDescription)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestActionAndAssertions<T1, T2> And(Expression<Action<T1, T2, TestActionResult>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestActionAndAssertions<T1, T2> And(Action<T1, T2, TestActionResult> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                testAction(prereqs.Item1, prereqs.Item2);
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        private void AddAssertion(Action<T1, T2, TestActionResult> assertion, string description)
        {
            // TODO: Try to avoid lambda here, too
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, invocationResult), description));
        }
    }

    /// <summary>
    /// Builder for providing additional assertions for a test with a three "Given" clauses
    /// and for which the "When" clause does not return a value.
    /// </summary>
    public class TestActionAndAssertions<T1, T2, T3> : ITest
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private TestActionResult invocationResult;

        internal TestActionAndAssertions((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction, Expression<Action<T1, T2, T3, TestActionResult>> assertion)
            : this(prereqs, testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction, Action<T1, T2, T3, TestActionResult> assertion, string assertionDescription)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestActionAndAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, TestActionResult>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestActionAndAssertions<T1, T2, T3> And(Action<T1, T2, T3, TestActionResult> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            if (invocationResult != null)
            {
                throw new InvalidOperationException("Test action already invoked");
            }

            try
            {
                testAction(prereqs.Item1, prereqs.Item2, prereqs.Item3);
                invocationResult = new TestActionResult();
            }
            catch (Exception e)
            {
                invocationResult = new TestActionResult(e);
            }
        }

        private void AddAssertion(Action<T1, T2, T3, TestActionResult> assertion, string description)
        {
            // TODO: Try to avoid lambda here, too
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, prereqs.Item3, invocationResult), description));
        }
    }
}
