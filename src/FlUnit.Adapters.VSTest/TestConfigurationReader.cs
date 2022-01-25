using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Reader logic for <see cref="TestConfiguration"/> instances.
    /// </summary>
    public static class TestConfigurationReader
    {
        /// <summary>
        /// Reads and returns a <see cref="TestConfiguration"/> instance from an XML reader.
        /// </summary>
        /// <param name="reader">A reader positioned at the root element of the test configuration (this class doesn't care what it is called).</param>
        /// <returns>A new <see cref="TestConfiguration"/> instance.</returns>
        public static TestConfiguration ReadXml(XmlReader reader)
        {
            TestConfiguration configuration = new TestConfiguration();

            if (!reader.TryReadToFirstChildElement())
            {
                return configuration;
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsAtElementWithName(nameof(TestConfiguration.FailedArrangementOutcomeIsSkipped)))
                {
                    reader.TryReadBoolean(b => configuration.FailedArrangementOutcomeIsSkipped = b);
                }
                else
                {
                    reader.Skip();
                }
            }

            return configuration;
        }
    }
}
