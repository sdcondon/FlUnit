using System;

namespace FlUnit
{

	/// <summary>
    /// Builder for providing additional pre-requisites for a test.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
	public sealed class TestBuilderWithPrerequisites<T1>
	{
		private readonly Func<T1> arrange;

		internal TestBuilderWithPrerequisites(Func<T1> prereqs) => this.arrange = prereqs;

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T2">The type of the pre-requisite.</typeparam>
        /// <param name="prereq2">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2> And<T2>(Func<T2> prereq2)
        {
            return new TestBuilderWithPrerequisites<T1, T2>((arrange, prereq2));
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1> When(Action<T1> testAction)
        {
            return new TestBuilderWithAction<T1>(arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, TResult> When<TResult>(Func<T1, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, TResult>(arrange, testFunction);
        }
	}

	/// <summary>
    /// Builder for providing additional pre-requisites for a test.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
	public sealed class TestBuilderWithPrerequisites<T1, T2>
	{
		private readonly (Func<T1>, Func<T2>) arrange;

		internal TestBuilderWithPrerequisites((Func<T1>, Func<T2>) prereqs) => this.arrange = prereqs;

        /// <summary>
        /// Adds another "Given" clause for the test.
        /// </summary>
        /// <typeparam name="T3">The type of the pre-requisite.</typeparam>
        /// <param name="prereq3">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or the "When" clause for the test.</returns>
        public TestBuilderWithPrerequisites<T1, T2, T3> And<T3>(Func<T3> prereq3)
        {
            return new TestBuilderWithPrerequisites<T1, T2, T3>((arrange.Item1, arrange.Item2, prereq3));
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1, T2> When(Action<T1, T2> testAction)
        {
            return new TestBuilderWithAction<T1, T2>(arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, T2, TResult> When<TResult>(Func<T1, T2, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, T2, TResult>(arrange, testFunction);
        }
	}

	/// <summary>
    /// Builder for providing additional pre-requisites for a test.
    /// </summary>
    /// <typeparam name="T1">The type of the 1st pre-requisite already defined.</typeparam>
    /// <typeparam name="T2">The type of the 2nd pre-requisite already defined.</typeparam>
    /// <typeparam name="T3">The type of the 3rd pre-requisite already defined.</typeparam>
	public sealed class TestBuilderWithPrerequisites<T1, T2, T3>
	{
		private readonly (Func<T1>, Func<T2>, Func<T3>) arrange;

		internal TestBuilderWithPrerequisites((Func<T1>, Func<T2>, Func<T3>) prereqs) => this.arrange = prereqs;


        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testAction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithAction<T1, T2, T3> When(Action<T1, T2, T3> testAction)
        {
            return new TestBuilderWithAction<T1, T2, T3>(arrange, testAction);
        }

        /// <summary>
        /// Adds a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public TestBuilderWithFunction<T1, T2, T3, TResult> When<TResult>(Func<T1, T2, T3, TResult> testFunction)
        {
            return new TestBuilderWithFunction<T1, T2, T3, TResult>(arrange, testFunction);
        }
	}
}