using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flutter
{
    public class TestThat
    {
        public static TestPrerequisite<T> Given<T>(T prerequisite) => new TestPrerequisite<T>(prerequisite);
    }

    public interface ITest
    {
        Task Invocation { get; }
    }

    public class Test<T1, TResult> : ITest
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;
        private readonly List<Action<T1, Task<TResult>>> assertions;
        internal Test(T1 prereq, Func<T1, TResult> testFunction, Action<T1, Task<TResult>> assertion) => (this.prereq, this.testFunction, this.assertions) = (prereq, testFunction, new List<Action<T1, Task<TResult>>> { assertion });
        public Test<T1, TResult> And(Action<T1, Task<TResult>> assertion) { assertions.Add(assertion); return this; }
        public Task Invocation => null;
    }

    public class Test<T1, T2, TResult> : ITest
    {
        private (T1, T2) prereqs;
        private Func<T1, T2, TResult> testFunction;
        private List<Action<T1, T2, Task<TResult>>> assertions;
        internal Test((T1, T2) prereqs, Func<T1, T2, TResult> testFunction, Action<T1, T2, Task<TResult>> assertion) => (this.prereqs, this.testFunction, this.assertions) = (prereqs, testFunction, new List<Action<T1, T2, Task<TResult>>> { assertion });
        public Test<T1, T2, TResult> And(Action<T1, T2, Task<TResult>> assertion) { assertions.Add(assertion); return this; }
        public Task Invocation => null;
    }

    public class Test<T1, T2, T3, TResult> : ITest
    {
        private (T1, T2, T3) prereqs;
        private Func<T1, T2, T3, TResult> testFunction;
        private List<Action<T1, T2, T3, Task<TResult>>> tests;
        internal Test((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction, Action<T1, T2, T3, Task<TResult>> test) => (this.prereqs, this.testFunction, this.tests) = (prereqs, testFunction, new List<Action<T1, T2, T3, Task<TResult>>> { test });
        public Test<T1, T2, T3, TResult> And(Action<T1, T2, T3, Task<TResult>> test) { tests.Add(test); return this; }
        public Task Invocation => null;
    }
}
