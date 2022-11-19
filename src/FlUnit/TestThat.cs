using FlUnit.Configuration;
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
        /// Starts building a test by providing an override to be applied to the test configuration used by the test runner (generally by being read from a configuration file - .runsettings, in the case of VSTest).
        /// </summary>
        /// <param name="configurationOverride">The override to apply.</param>
        /// <returns>A builder for providing "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder UsingConfiguration(Action<ITestConfiguration> configurationOverride) => new TestPrerequisitesBuilder(new[] { configurationOverride });

        /// <summary>
        /// Starts building a test by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisite">A delegate to create the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> Given<T>(Func<T> prerequisite) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).Given(prerequisite);

        /// <summary>
        /// Starts building a test by providing a "Given" clause, makng use of the test context.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisite">A delegate to create the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> Given<T>(Func<ITestContext, T> prerequisite) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).Given(prerequisite);

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisites">A delegate to create the prerequisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenEachOf<T>(Func<IEnumerable<T>> prerequisites) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenEachOf(prerequisites);

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause, making use of the test context.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisites">A delegate to create the prerequisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenEachOf<T>(Func<ITestContext, IEnumerable<T>> prerequisites) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenEachOf(prerequisites);

        /// <summary>
        /// Starts building a test by defining the test context as the first prerequisite. This is just a more readable alias of:
        /// <code>
        /// Given(ctx => ctx)
        /// </code>
        /// </summary>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<ITestContext> GivenTestContext() => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenTestContext();

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that does not return a value.
        /// </summary>
        /// <param name="testAction">The action that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static ActionTestBuilder When(Action testAction) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).When(testAction);

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static FunctionTestBuilder<T> When<T>(Func<T> testFunction) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).When(testFunction);
    }
}
