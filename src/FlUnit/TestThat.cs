using System;

namespace FlUnit
{
    public class TestThat
    {
        public static TestPrerequisite<T> Given<T>(T prerequisite) => new TestPrerequisite<T>(prerequisite);

        //public static IEnumerable<TestPrerequisite<T>> GivenEachOf<T>(IEnumerable<T> prerequisites) => prerequisites.Select(p => Given(p));

        public static TestAction When(Action testAction) => new TestAction(testAction);

        public static TestFunction<T> When<T>(Func<T> testFunction) => new TestFunction<T>(testFunction);
    }
}
