using FlUnit.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static TestPrerequisitesBuilder UsingConfiguration(Action<ITestConfiguration> configurationOverride) => new(new[] { configurationOverride });

        /// <summary>
        /// Starts building a test by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisiteGetter">A delegate to create the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> Given<T>(Func<T> prerequisiteGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).Given(prerequisiteGetter);

        /// <summary>
        /// Starts building a test by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisiteGetter">A delegate to create the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenAsync<T>(Func<Task<T>> prerequisiteGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenAsync(prerequisiteGetter);

        /// <summary>
        /// Starts building a test by providing a "Given" clause, making use of the test context.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisiteGetter">A delegate to create the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> Given<T>(Func<ITestContext, T> prerequisiteGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).Given(prerequisiteGetter);

        /// <summary>
        /// Starts building a test by providing a "Given" clause, making use of the test context.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisiteGetter">A delegate to create the prerequisite.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenAsync<T>(Func<ITestContext, Task<T>> prerequisiteGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenAsync(prerequisiteGetter);

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisitesGetter">A delegate to create the prerequisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenEachOf<T>(Func<IEnumerable<T>> prerequisitesGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenEachOf(prerequisitesGetter);

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisitesGetter">A delegate to create the prerequisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenEachOfAsync<T>(Func<Task<IEnumerable<T>>> prerequisitesGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenEachOfAsync(prerequisitesGetter);

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause, making use of the test context.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisitesGetter">A delegate to create the prerequisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenEachOf<T>(Func<ITestContext, IEnumerable<T>> prerequisitesGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenEachOf(prerequisitesGetter);

        /// <summary>
        /// Starts building a test with multiple cases by providing a "Given" clause, making use of the test context.
        /// </summary>
        /// <typeparam name="T">The type of the prerequisite.</typeparam>
        /// <param name="prerequisitesGetter">A delegate to create the prerequisites - one for each test case.</param>
        /// <returns>A builder for providing more "Given" clauses or a "When" clause.</returns>
        public static TestPrerequisitesBuilder<T> GivenEachOfAsync<T>(Func<ITestContext, Task<IEnumerable<T>>> prerequisitesGetter) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).GivenEachOfAsync(prerequisitesGetter);

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
        /// Starts building a test with no "Given" clauses and a "When" clause that does not return a value.
        /// </summary>
        /// <param name="asyncTestAction">The asynchronous action that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static ActionTestBuilder WhenAsync(Func<Task> asyncTestAction) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).WhenAsync(asyncTestAction);

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that returns a value.
        /// </summary>
        /// <param name="testFunction">The function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static FunctionTestBuilder<T> When<T>(Func<T> testFunction) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).When(testFunction);

        /// <summary>
        /// Starts building a test with no "Given" clauses and a "When" clause that returns a value.
        /// </summary>
        /// <param name="asyncTestFunction">The asynchronous function that is the "When" clause of the test.</param>
        /// <returns>A builder for providing "Then" clauses.</returns>
        public static FunctionTestBuilder<T> WhenAsync<T>(Func<Task<T>> asyncTestFunction) => new TestPrerequisitesBuilder(Array.Empty<Action<ITestConfiguration>>()).WhenAsync(asyncTestFunction);
    }
}
