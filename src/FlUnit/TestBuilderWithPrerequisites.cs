using System;

namespace FlUnit
{
    //// TODO: T4 template me!!

    /// <summary>
    /// Builder for providing additional pre-requisites for a test.
    /// </summary>
    /// <typeparam name="T1">The type of the single pre-requisite already defined.</typeparam>
    public sealed class TestBuilderWithPrerequisites<T1>
    {
        private readonly T1 prereq;

        internal TestBuilderWithPrerequisites(T1 prereq) => this.prereq = prereq;

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereq2">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2> And<T2>(T2 prereq2)
        {
            return new TestBuilderWithPrerequisites<T1, T2>((prereq, prereq2));
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1> When(Action<T1> testAction)
        {
            return new TestBuilderWithAction<T1>(prereq, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, TResult> When<TResult>(Func<T1, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, TResult>(prereq, testFunction);
        }
    }

    /// <summary>
    /// Builder for providing additional pre-requisites for a test.
    /// </summary>
    /// <typeparam name="T1">The type of the first pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the second pre-requisite already defined.</typeparam>
    public sealed class TestBuilderWithPrerequisites<T1, T2>
    {
        private readonly (T1, T2) prereqs;

        internal TestBuilderWithPrerequisites((T1, T2) prereqs) => this.prereqs = prereqs;

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereq3">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2, T3> And<T3>(T3 prereq3)
        {
            return new TestBuilderWithPrerequisites<T1, T2, T3>((prereqs.Item1, prereqs.Item2, prereq3));
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1, T2> When(Action<T1, T2> testAction)
        {
            return new TestBuilderWithAction<T1, T2>(prereqs, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, T2, TResult> When<TResult>(Func<T1, T2, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, T2, TResult>(prereqs, testFunction);
        }
    }

    /// <summary>
    /// Builder for providing additional pre-requisites for a test.
    /// </summary>
    /// <typeparam name="T1">The type of the first pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the second pre-requisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the third pre-requisite already defined.</typeparam>
    public sealed class TestBuilderWithPrerequisites<T1, T2, T3>
    {
        private readonly (T1, T2, T3) prereqs;

        internal TestBuilderWithPrerequisites((T1, T2, T3) prereqs) => this.prereqs = prereqs;

        //public TestPrerequisite<T1, T2, T3, T4> And<T4>(T4 prereq4)
        //{
        //    return new TestPrerequisite<T1, T2, T3, T4>((prereqs.Item1, prereqs.Item2, prereqs.Item3, prereq4));
        //}

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1, T2, T3> When(Action<T1, T2, T3> testAction)
        {
            return new TestBuilderWithAction<T1, T2, T3>(prereqs, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, T2, T3, TResult> When<TResult>(Func<T1, T2, T3, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, T2, T3, TResult>(prereqs, testFunction);
        }
    }
}
