using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FlUnit
{
    //// TODO: T4 template me!!

    public class TestFunction<TResult>
    {
        private readonly Func<TResult> testFunction;

        internal TestFunction(Func<TResult> testFunction)
        {
            this.testFunction = testFunction;
        }

        public TestFunctionAndAssertions<TResult> Then(Expression<Action<Task<TResult>>> assertion)
        {
            return new TestFunctionAndAssertions<TResult>(testFunction, assertion);
        }

        public TestFunctionAndAssertions<TResult> Then(Action<Task<TResult>> assertion, string description = null)
        {
            return new TestFunctionAndAssertions<TResult>(testFunction, assertion, description);
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

        public TestFunctionAndAssertions<T1, TResult> Then(Expression<Action<T1, Task<TResult>>> assertion)
        {
            return new TestFunctionAndAssertions<T1, TResult>(prereq, testFunction, assertion);
        }

        public TestFunctionAndAssertions<T1, TResult> Then(Action<T1, Task<TResult>> assertion, string description = null)
        {
            return new TestFunctionAndAssertions<T1, TResult>(prereq, testFunction, assertion, description);
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

        public TestFunctionAndAssertions<T1, T2, TResult> Then(Expression<Action<T1, T2, Task<TResult>>> assertion)
        {
            return new TestFunctionAndAssertions<T1, T2, TResult>(prereqs, testFunction, assertion);
        }

        public TestFunctionAndAssertions<T1, T2, TResult> Then(Action<T1, T2, Task<TResult>> assertion, string description = null)
        {
            return new TestFunctionAndAssertions<T1, T2, TResult>(prereqs, testFunction, assertion, description);
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

        public TestFunctionAndAssertions<T1, T2, T3, TResult> Then(Expression<Action<T1, T2, T3, Task<TResult>>> assertion)
        {
            return new TestFunctionAndAssertions<T1, T2, T3, TResult>(prereqs, testFunction, assertion);
        }

        public TestFunctionAndAssertions<T1, T2, T3, TResult> Then(Action<T1, T2, T3, Task<TResult>> assertion, string description = null)
        {
            return new TestFunctionAndAssertions<T1, T2, T3, TResult>(prereqs, testFunction, assertion, description);
        }
    }
}
