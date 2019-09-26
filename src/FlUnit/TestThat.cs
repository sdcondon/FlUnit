namespace FlUnit
{
    public class TestThat
    {
        public static TestPrerequisite<T> Given<T>(T prerequisite) => new TestPrerequisite<T>(prerequisite);
    }
}
