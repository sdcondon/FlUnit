namespace FlUnit.Adapters
{
    /// <summary>
    /// Container for configuration that affects a test run as a whole
    /// (as opposed to configuration that is relevant to individual tests within that run).
    /// </summary>
    public class TestRunConfiguration
    {
        /// <summary>
        /// Gets the subset of run configuration that is relevant to (and ultimately overridable by) individual tests.
        /// </summary>
        public TestConfiguration TestConfiguration { get; set; } = new TestConfiguration();

        /// <summary>
        /// Gets or set a value indicating whether tests should be run in parallel or not.
        /// The default value is 'true'.
        /// </summary>
        public bool Parallelise { get; set; } = true;

        /////// <summary>
        /////// Gets or sets the trait name to control partitioning of tests run in parallel.
        /////// Tests with the same value for the trait with this key will be executed in the same partition, and thus not run in parallel.
        /////// </summary>
        ////public bool ParallelPartitionKey { get; set; }
    }
}
