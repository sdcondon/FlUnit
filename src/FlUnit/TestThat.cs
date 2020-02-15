using System;

namespace FlUnit
{
    /// <summary>
    /// Entry point for building <see cref="ITest"/> instances.
    /// </summary>
    public class TestThat
    {
        /// <summary>
        /// Starts building a test by providing an arrangment step.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prerequisite"></param>
        /// <returns></returns>
        public static TestPrerequisite<T> Given<T>(T prerequisite) => new TestPrerequisite<T>(prerequisite);

        //public static IEnumerable<TestPrerequisite<T>> GivenEachOf<T>(IEnumerable<T> prerequisites) => prerequisites.Select(p => Given(p));

        /// <summary>
        /// Starts building a test with no arrangement steps.
        /// </summary>
        /// <param name="testAction"></param>
        /// <returns></returns>
        public static TestAction When(Action testAction) => new TestAction(testAction);

        public static TestFunction<T> When<T>(Func<T> testFunction) => new TestFunction<T>(testFunction);
    }
}
