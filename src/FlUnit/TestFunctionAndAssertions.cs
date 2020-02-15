using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlUnit
{
    //// TODO: T4 template me!!

    public class TestFunctionAndAssertions<TResult> : ITest
    {
        private readonly Func<TResult> invoke;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task<TResult> invocationResult;

        internal TestFunctionAndAssertions(Func<TResult> testFunction, Expression<Action<Task<TResult>>> assertion)
            : this(testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions(Func<TResult> testFunction, Action<Task<TResult>> assertion, string assertionDescription)
        {
            this.invoke = testFunction;
            AddAssertion(assertion, assertionDescription);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<TResult> And(Expression<Action<Task<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<TResult> And(Action<Task<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            invocationResult = new Task<TResult>(invoke);
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<Task<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(invocationResult), description));
        }
    }

    public class TestFunctionAndAssertions<T1, TResult> : ITest
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task<TResult> invocationResult;

        internal TestFunctionAndAssertions(T1 prereq, Func<T1, TResult> testFunction, Expression<Action<T1, Task<TResult>>> assertion)
            : this(prereq, testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions(T1 prereq, Func<T1, TResult> testFunction, Action<T1, Task<TResult>> assertion, string description)
        {
            this.prereq = prereq;
            this.testFunction = testFunction;
            AddAssertion(assertion, description);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<T1, TResult> And(Expression<Action<T1, Task<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<T1, TResult> And(Action<T1, Task<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            // TODO: Exceptions are gonna be aggregates...
            invocationResult = new Task<TResult>(() => testFunction(prereq));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, Task<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereq, invocationResult), description));
        }
    }

    public class TestFunctionAndAssertions<T1, T2, TResult> : ITest
    {
        private readonly (T1, T2) prereqs;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task<TResult> invocationResult;

        internal TestFunctionAndAssertions((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Expression<Action<T1, T2, Task<TResult>>> assertion)
            : this(prereqs, testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Action<T1, T2, Task<TResult>> assertion, string description)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            AddAssertion(assertion, description);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<T1, T2, TResult> And(Expression<Action<T1, T2, Task<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<T1, T2, TResult> And(Action<T1, T2, Task<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereqs.Item1, prereqs.Item2));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, T2, Task<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, invocationResult), description));
        }
    }

    public class TestFunctionAndAssertions<T1, T2, T3, TResult> : ITest
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<TestAssertion> assertions = new List<TestAssertion>();
        private Task<TResult> invocationResult;

        internal TestFunctionAndAssertions((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Expression<Action<T1, T2, T3, Task<TResult>>> assertion)
            : this(prereqs, testFunction, assertion.Compile(), assertion.Body.ToString())
        {
        }

        internal TestFunctionAndAssertions((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Action<T1, T2, T3, Task<TResult>> assertion, string description)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            AddAssertion(assertion, description);
        }

        public IEnumerable<TestAssertion> Assertions => assertions;

        public TestFunctionAndAssertions<T1, T2, T3, TResult> And(Expression<Action<T1, T2, T3, Task<TResult>>> assertion)
        {
            return And(assertion.Compile(), assertion.Body.ToString());
        }

        public TestFunctionAndAssertions<T1, T2, T3, TResult> And(Action<T1, T2, T3, Task<TResult>> assertion, string description)
        {
            AddAssertion(assertion, description);
            return this;
        }

        public void Act()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereqs.Item1, prereqs.Item2, prereqs.Item3));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, T2, T3, Task<TResult>> assertion, string description)
        {
            assertions.Add(new TestAssertion(() => assertion(prereqs.Item1, prereqs.Item2, prereqs.Item3, invocationResult), description));
        }
    }
}
