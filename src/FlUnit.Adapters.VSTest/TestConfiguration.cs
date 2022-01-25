using FlUnit.Configuration;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Container for settings that affect individual tests (as opposed to settings that are relevant to a run as a whole).
    /// These are the settings that it will ultimately be possible to override in individual tests.
    /// </summary>
    public class TestConfiguration : ITestConfiguration
    {
        /// <inheritdoc />
        public bool FailedArrangementOutcomeIsSkipped { get; set; } = true;

        /// <inheritdoc />
        public IResultNamingStrategy ResultNamingStrategy { get; set; } = new DefaultResultNamingStrategy();
    }
}
