namespace FlUnit.Adapters
{
    /// <summary>
    /// Container for configuration that affects a test run as a whole
    /// (as opposed to configuration that is relevant to individual tests within that run).
    /// </summary>
    public class TestRunConfiguration
    {
        /// <summary>
        /// Gets the default run settings. These are the setting values used if not otherwise specified.
        /// </summary>
        public static TestRunConfiguration Default { get; } = new TestRunConfiguration();

        /// <summary>
        /// Gets the subset of run configuration that is relevant to (and ultimately overridable by) individual tests.
        /// </summary>
        public TestConfiguration TestConfiguration { get; set; } = TestConfiguration.Default;

        //// E.g. Parallelisation control settings go in here.
    }
}
