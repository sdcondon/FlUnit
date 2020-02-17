using System;

namespace FlUnit
{
    /// <summary>
    /// Entry point for building <see cref="ITest"/> instances.
    /// </summary>
    public class TestThat
    {
        /// <summary>
        /// Starts building a test by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the pre-requisite.</typeparam>
        /// <param name="prerequisite">The pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisite<T> Given<T>(T prerequisite) => new TestPrerequisite<T>(prerequisite);

        ////public static IEnumerable<TestPrerequisite<T>> GivenEachOf<T>(IEnumerable<T> prerequisites) => prerequisites.Select(p => Given(p));

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static TestAction When(Action testAction) => new TestAction(testAction);

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static TestFunction<T> When<T>(Func<T> testFunction) => new TestFunction<T>(testFunction);
    }
}
