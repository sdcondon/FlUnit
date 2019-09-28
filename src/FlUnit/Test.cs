using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlUnit
{
    public interface ITest
    {
        void Run();

        IEnumerable<Action> Assertions { get; }
    }

    public class Test<TResult> : ITest
    {
        private readonly Func<TResult> testFunction;
        private readonly List<Action<Task<TResult>>> assertions;

        private Task<TResult> invocationResult;

        internal Test(Func<TResult> testFunction, Action<Task<TResult>> assertion)
        {
            this.testFunction = testFunction;
            this.assertions = new List<Action<Task<TResult>>> { assertion };
        }

        public Test<TResult> And(Action<Task<TResult>> assertion)
        {
            assertions.Add(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(testFunction);
            invocationResult.RunSynchronously();
        }

        public IEnumerable<Action> Assertions => assertions.Select(a => (Action)(() => a(invocationResult)));
    }

    public class Test<T1, TResult> : ITest
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<Action<T1, Task<TResult>>> assertions;

        private Task<TResult> invocationResult;

        internal Test(T1 prereq, Func<T1, TResult> testFunction, Action<T1, Task<TResult>> assertion)
        {
            this.prereq = prereq;
            this.testFunction = testFunction;
            this.assertions = new List<Action<T1, Task<TResult>>> { assertion };
        }

        public Test<T1, TResult> And(Action<T1, Task<TResult>> assertion)
        {
            assertions.Add(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereq));
            invocationResult.RunSynchronously();
        }

        public IEnumerable<Action> Assertions => assertions.Select(a => (Action)(() => a(prereq, invocationResult)));
    }

    public class Test<T1, T2, TResult> : ITest
    {
        private readonly (T1, T2) prereqs;
        private readonly Func<T1, T2, TResult> testFunction;
        private readonly List<Action<T1, T2, Task<TResult>>> assertions;

        private Task<TResult> invocationResult;

        internal Test((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Action<T1, T2, Task<TResult>> assertion)
        {
            (this.prereqs, this.testFunction, this.assertions) = (prereqs, testFunction, new List<Action<T1, T2, Task<TResult>>> { assertion });
        }

        public Test<T1, T2, TResult> And(Action<T1, T2, Task<TResult>> assertion)
        {
            assertions.Add(assertion);
            return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereqs.Item1, prereqs.Item2));
            invocationResult.RunSynchronously();
        }

        public IEnumerable<Action> Assertions => assertions.Select(a => (Action)(() => a(prereqs.Item1, prereqs.Item2, invocationResult)));
    }

    public class Test<T1, T2, T3, TResult> : ITest
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Func<T1, T2, T3, TResult> testFunction;
        private readonly List<Action<T1, T2, T3, Task<TResult>>> assertions;

        private Task<TResult> invocationResult;

        internal Test((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Action<T1, T2, T3, Task<TResult>> test)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
            this.assertions = new List<Action<T1, T2, T3, Task<TResult>>> { test };
        }

        public Test<T1, T2, T3, TResult> And(Action<T1, T2, T3, Task<TResult>> test)
        {
            assertions.Add(test); return this;
        }

        public void Run()
        {
            invocationResult = new Task<TResult>(() => testFunction(prereqs.Item1, prereqs.Item2, prereqs.Item3));
            invocationResult.RunSynchronously();
        }

        public IEnumerable<Action> Assertions => assertions.Select(a => (Action)(() => a(prereqs.Item1, prereqs.Item2, prereqs.Item3, invocationResult)));
    }
}
