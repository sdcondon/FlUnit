using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlUnit
{
    public interface ITest
    {
        void Run();
        IEnumerable<(Action, string)> Assertions { get; }
    }

    public class Test<TResult> : ITest
    {
        private readonly Func<TResult> invoke;
        private readonly List<(Action, string)> assertions = new List<(Action, string)>();
        private Task<TResult> invocationResult;

        internal Test(Func<TResult> testFunction, Action<Task<TResult>> assertion)
        {
            this.invoke = testFunction;
            AddAssertion(assertion);
        }

        public IEnumerable<(Action, string)> Assertions => assertions;

        public Test<TResult> And(Action<Task<TResult>> assertion)
        {
            AddAssertion(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(invoke);
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<Task<TResult>> assertion)
        {
            assertions.Add(((Action)(() => assertion(invocationResult)), ""));
        }
    }

    public class Test<T1, TResult> : ITest
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<(Action, string)> assertions = new List<(Action, string)>();
        private Task<TResult> invocationResult;

        internal Test(T1 prereq, Func<T1, TResult> testFunction, Action<T1, Task<TResult>> assertion)
        {
            this.prereq = prereq;
            this.testFunction = testFunction;
            AddAssertion(assertion);
        }

        public IEnumerable<(Action, string)> Assertions => assertions;

        public Test<T1, TResult> And(Action<T1, Task<TResult>> assertion)
        {
            AddAssertion(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereq));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, Task<TResult>> assertion)
        {
            assertions.Add(((Action)(() => assertion(prereq, invocationResult)), ""));
        }
    }

    public class Test<T1, T2, TResult> : ITest
    {
        private readonly (T1, T2) prereqs;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<(Action, string)> assertions = new List<(Action, string)>();
        private Task<TResult> invocationResult;

        internal Test((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Action<T1, T2, Task<TResult>> assertion)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            AddAssertion(assertion);
        }

        public IEnumerable<(Action, string)> Assertions => assertions;

        public Test<T1, T2, TResult> And(Action<T1, T2, Task<TResult>> assertion)
        {
            AddAssertion(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereqs.Item1, prereqs.Item2));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, T2, Task<TResult>> assertion)
        {
            assertions.Add(((Action)(() => assertion(prereqs.Item1, prereqs.Item2, invocationResult)), ""));
        }
    }

    public class Test<T1, T2, T3, TResult> : ITest
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<(Action, string)> assertions = new List<(Action, string)>();
        private Task<TResult> invocationResult;

        internal Test((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Action<T1, T2, T3, Task<TResult>> assertion)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            AddAssertion(assertion);
        }

        public IEnumerable<(Action, string)> Assertions => assertions;

        public Test<T1, T2, T3, TResult> And(Action<T1, T2, T3, Task<TResult>> assertion)
        {
            AddAssertion(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereqs.Item1, prereqs.Item2, prereqs.Item3));
            invocationResult.RunSynchronously();
        }

        private void AddAssertion(Action<T1, T2, T3, Task<TResult>> assertion)
        {
            assertions.Add(((Action)(() => assertion(prereqs.Item1, prereqs.Item2, prereqs.Item3, invocationResult)), ""));
        }
    }
}
