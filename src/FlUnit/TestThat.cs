using System;
using System.Collections.Generic;

namespace FlUnit
{
    /// <summary>
    /// Entry point for building <see cref="Test"/> instances.
    /// </summary>
    public static class TestThat
    {
        /// <summary>
        /// Starts building a test by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the pre-requisite.</typeparam>
        /// <param name="prerequisite">A delegate to create the pre-requisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestBuilderWithPrerequisites<T> Given<T>(Func<T> prerequisite) => new TestBuilderWithPrerequisites<T>(() => new[] { prerequisite() });

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the pre-requisite.</typeparam>
        /// <param name="prerequisites">A delegate to create the pre-requisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestBuilderWithPrerequisites<T> GivenEachOf<T>(Func<IEnumerable<T>> prerequisites) => new TestBuilderWithPrerequisites<T>(prerequisites);

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static TestBuilderWithAction When(Action testAction) => new TestBuilderWithAction(testAction);

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static TestBuilderWithFunction<T> When<T>(Func<T> testFunction) => new TestBuilderWithFunction<T>(testFunction);
    }
}
