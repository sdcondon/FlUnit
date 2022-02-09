using FlUnit.Configuration;
using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Container for settings that affect individual tests (as opposed to settings that are relevant to a run as a whole).
    /// These are the settings that it will ultimately be possible to override in individual tests.
    /// </summary>
    public class TestConfiguration : ITestConfiguration
    {
        /// <inheritdoc />
        public bool ArrangementFailureCountsAsFailed { get; set; } = false;

        /// <inheritdoc />
        public IResultNamingStrategy ResultNamingStrategy { get; set; } = new DefaultResultNamingStrategy();

        /// <summary>
        /// Reads and returns a <see cref="TestConfiguration"/> instance from an XML reader.
        /// </summary>
        /// <param name="reader">A reader positioned at the root element of the test configuration (this class doesn't care what the element is called).</param>
        /// <returns>A new <see cref="TestConfiguration"/> instance.</returns>
        public static TestConfiguration ReadFromXml(XmlReader reader)
        {
            TestConfiguration configuration = new TestConfiguration();

            if (reader.TryReadToFirstChildElement())
            {
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.IsAtElementWithName(nameof(ArrangementFailureCountsAsFailed)))
                    {
                        reader.TryReadBoolean(b => configuration.ArrangementFailureCountsAsFailed = b);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }

            return configuration;
        }

        /// <summary>
        /// Clones this test configuration object.
        /// </summary>
        /// <returns>A new instance that is a copy of this one.</returns>
        public TestConfiguration Clone()
        {
            return new TestConfiguration()
            {
                ArrangementFailureCountsAsFailed = ArrangementFailureCountsAsFailed,
                ResultNamingStrategy = ResultNamingStrategy,
            };
        }
    }
}
