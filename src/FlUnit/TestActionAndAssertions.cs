using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlUnit
{
    public class TestActionAndAssertions : ITest
    {
        private readonly Action invoke;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task invocationResult;

        internal TestActionAndAssertions(Action testAction, Expression<Action<Task>> assertion)
            : this(testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions(Action testAction, Action<Task> assertion, string assertionDescription)
        {
            this.invoke = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        //public TestActionAndAssertions And(Expression<Action<Task>> assertion)
        //{
        //    return And(assertion.Compile(), assertion.Body.ToString());
        //}

        public TestActionAndAssertions And(Action<Task> assertion, string description = null)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task(invoke);
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<Task> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(invocationResult), description));
        }
    }

    public class TestActionAndAssertions<T1> : ITest
    {
        private readonly T1 prereq;
        private readonly Action<T1> testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task invocationResult;

        internal TestActionAndAssertions(T1 prereq, Action<T1> testAction, Expression<Action<T1, Task>> assertion)
            : this(prereq, testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions(T1 prereq, Action<T1> testAction, Action<T1, Task> assertion, string assertionDescription)
        {
            this.prereq = prereq;
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        //public TestActionAndAssertions<T1> And(Expression<Action<T1, Task>> assertion)
        //{
        //    return And(assertion.Compile(), assertion.Body.ToString());
        //}

        public TestActionAndAssertions<T1> And(Action<T1, Task> assertion, string description = null)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task(() => testAction(prereq));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, Task> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereq, invocationResult), description));
        }
    }

    public class TestActionAndAssertions<T1, T2> : ITest
    {
        private readonly (T1, T2) prereqs;
        private readonly Action<T1, T2> testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task invocationResult;

        internal TestActionAndAssertions((T1, T2) prereqs, Action<T1, T2> testAction, Expression<Action<T1, T2, Task>> assertion)
            : this(prereqs, testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions((T1, T2) prereqs, Action<T1, T2> testAction, Action<T1, T2, Task> assertion, string assertionDescription)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestActionAndAssertions<T1, T2> And(Expression<Action<T1, T2, Task>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestActionAndAssertions<T1, T2> And(Action<T1, T2, Task> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task(() => testAction(prereqs.Item1, prereqs.Item2));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, T2, Task> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, invocationResult), description));
        }
    }

    public class TestActionAndAssertions<T1, T2, T3> : ITest
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Action<T1, T2, T3> testAction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task invocationResult;

        internal TestActionAndAssertions((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction, Expression<Action<T1, T2, T3, Task>> assertion)
            : this(prereqs, testAction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestActionAndAssertions((T1, T2, T3) prereqs, Action<T1, T2, T3> testAction, Action<T1, T2, T3, Task> assertion, string assertionDescription)
        {
            this.prereqs = prereqs;
            this.testAction = testAction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        //public TestActionAndAssertions<T1, T2, T3> And(Expression<Action<T1, T2, T3, Task>> assertion)
        //{
        //    return And(assertion.Compile(), assertion.Body.ToString());
        //}

        public TestActionAndAssertions<T1, T2, T3> And(Action<T1, T2, T3, Task> assertion, string description = null)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task(() => testAction(prereqs.Item1, prereqs.Item2, prereqs.Item3));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, T2, T3, Task> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, prereqs.Item3, invocationResult), description));
        }
    }
}
