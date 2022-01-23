using FlUnit.Adapters.Configuration;
using FlUnit.Configuration;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Container for settings that affect individual tests (as opposed to settings that are relevant to a run as a whole).
    /// These are the settings that it will ultimately be possible to override in individual tests.
    /// </summary>
    public class TestConfiguration
    {
        /// <summary>
        /// Gets or sets the outcome for the (singular) test result when test arrangement fails (that is, when one of the "Given" clauses throws).
        /// Defaults to <see cref="TestOutcome.Skipped" />.
        /// </summary>
        public TestOutcome ArrangementFailureOutcome { get; set; } = TestOutcome.Skipped;

        /// <summary>
        /// Gets or sets the strategy to use to label individual test results.
        /// </summary>
        public IResultNamingStrategy ResultNamingStrategy { get; set; } = new DefaultResultNamingStrategy();
    }
}
