namespace FlUnit.Configuration
{
    /// <summary>
    /// Interface for containers of settings that affect individual tests (as opposed to settings that are relevant to a run as a whole).
    /// These are the settings that it will ultimately be possible to override in individual tests.
    /// </summary>
    public interface ITestConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the outcome for the (singular) test result when test arrangement fails (that is, when one of the "Given" clauses throws)
        /// should be "Skipped" (as opposed to "Failed").
        /// </summary>
        bool FailedArrangementOutcomeIsSkipped { get; set; }

        /// <summary>
        /// Gets or sets the strategy to use to label individual test results.
        /// </summary>
        IResultNamingStrategy ResultNamingStrategy { get; set; }
    }
}
