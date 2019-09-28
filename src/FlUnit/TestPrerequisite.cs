using System;

namespace FlUnit
{
    public class TestPrerequisite<T1>
    {
        private readonly T1 prereq;

        internal TestPrerequisite(T1 prereq) => this.prereq = prereq;

        public TestPrerequisite<T1, T2> And<T2>(T2 prereq2)
        {
            return new TestPrerequisite<T1, T2>((prereq, prereq2));
        }

        public TestFunction<T1, TResult> When<TResult>(Func<T1, TResult> testFunction)
        {
            return new TestFunction<T1, TResult>(prereq, testFunction);
        }
    }

    public class TestPrerequisite<T1, T2>
    {
        private readonly (T1, T2) prereqs;

        internal TestPrerequisite((T1, T2) prereqs) => this.prereqs = prereqs;

        public TestPrerequisite<T1, T2, T3> And<T3>(T3 prereq3)
        {
            return new TestPrerequisite<T1, T2, T3>((prereqs.Item1, prereqs.Item2, prereq3));
        }

        public TestFunction<T1, T2, TResult> When<TResult>(Func<T1, T2, TResult> testFunction)
        {
            return new TestFunction<T1, T2, TResult>(prereqs, testFunction);
        }
    }

    public class TestPrerequisite<T1, T2, T3>
    {
        private readonly (T1, T2, T3) prereqs;

        internal TestPrerequisite((T1, T2, T3) prereqs) => this.prereqs = prereqs;

        //public TestPrerequisite<T1, T2, T3, T4> And<T4>(T4 prereq4)
        //{
        //    return new TestPrerequisite<T1, T2, T3, T4>((prereqs.Item1, prereqs.Item2, prereqs.Item3, prereq4));
        //}

        public TestFunction<T1, T2, T3, TResult> When<TResult>(Func<T1, T2, T3, TResult> testFunction)
        {
            return new TestFunction<T1, T2, T3, TResult>(prereqs, testFunction);
        }
    }
}
