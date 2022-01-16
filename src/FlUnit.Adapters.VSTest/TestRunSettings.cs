namespace FlUnit.Adapters
{
    /// <summary>
    /// Container for settings that affect a test run (as opposed to settings that are relevant to individual
    /// tests within that run).
    /// </summary>
    public class TestRunSettings
    {
        /// <summary>
        /// Gets the default run settings. These are the setting values used if not otherwise specified.
        /// </summary>
        public static TestRunSettings Default { get; } = new TestRunSettings();

        /// <summary>
        /// Gets the subset of run settings that are relevant to (and ultimately overridable by) individual tests.
        /// </summary>
        public TestSettings TestSettings { get; set; }

        //// E.g. Parallelisation control settings go in here.
    }
}
