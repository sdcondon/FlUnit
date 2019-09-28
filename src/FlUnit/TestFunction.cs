using System;
using System.Threading.Tasks;

namespace FlUnit
{
    public class TestFunction<TResult>
    {
        private readonly Func<TResult> testFunction;

        internal TestFunction(Func<TResult> testFunction)
        {
            this.testFunction = testFunction;
        }

        public Test<TResult> Then(Action<Task<TResult>> assertion)
        {
            return new Test<TResult>(testFunction, assertion);
        }
    }

    public class TestFunction<T1, TResult>
    {
        private readonly T1 prereq;
        private readonly Func<T1, TResult> testFunction;

        internal TestFunction(T1 prereq, Func<T1, TResult> testFunction)
        {
            this.prereq = prereq;
            this.testFunction = testFunction;
        }

        public Test<T1, TResult> Then(Action<T1, Task<TResult>> assertion)
        {
            return new Test<T1, TResult>(prereq, testFunction, assertion);
        }
    }

    public class TestFunction<T1, T2, TResult>
    {
        private readonly (T1, T2) prereqs;
        private readonly Func<T1, T2, TResult> testFunction;

        internal TestFunction((T1, T2) prereqs, Func<T1, T2, TResult> testFunction)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
        }

        public Test<T1, T2, TResult> Then(Action<T1, T2, Task<TResult>> assertion)
        {
            return new Test<T1, T2, TResult>(prereqs, testFunction, assertion);
        }
    }

    public class TestFunction<T1, T2, T3, TResult>
    {
        private readonly (T1, T2, T3) prereqs;
        private readonly Func<T1, T2, T3, TResult> testFunction;

        internal TestFunction((T1, T2, T3) prereqs, Func<T1, T2, T3, TResult> testFunction)
        {
            this.prereqs = prereqs;
            this.testFunction = testFunction;
        }

        public Test<T1, T2, T3, TResult> Then(Action<T1, T2, T3, Task<TResult>> assertion)
        {
            return new Test<T1, T2, T3, TResult>(prereqs, testFunction, assertion);
        }
    }
}
