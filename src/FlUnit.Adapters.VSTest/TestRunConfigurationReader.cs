using System.IO;
using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Reader logic for <see cref="TestRunConfiguration"/> instances.
    /// </summary>
    public static class TestRunConfigurationReader
    {
        /// <summary>
        /// Reads and returns a <see cref="TestRunConfiguration"/> instance from an XML string.
        /// </summary>
        /// <param name="xml">The XML to read, including a root element (this class doesn't care what its name is).</param>
        /// <returns>A new <see cref="TestRunConfiguration"/> instance, this will be a default instance if the argument is null or empty.</returns>
        public static TestRunConfiguration ReadXml(string xml, string elementName)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new TestRunConfiguration();
            }

            using (var stringReader = new StringReader(xml))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                // NB: finds first matching element, wherever it sits in the tree - not the one closest to the root.
                // Would be very surprised if this were ever an actual problem, so fine..
                while (!xmlReader.EOF && xmlReader.Read() && !xmlReader.IsAtElementWithName(elementName))
                {
                }

                return ReadXml(xmlReader);
            }
        }

        /// <summary>
        /// Reads and returns a <see cref="TestRunConfiguration"/> instance from an XML reader.
        /// </summary>
        /// <param name="reader">A reader positioned at the root element of the test run configuration.</param>
        /// <returns>A new <see cref="TestRunConfiguration"/> instance.</returns>
        public static TestRunConfiguration ReadXml(XmlReader reader)
        {
            TestRunConfiguration configuration = new TestRunConfiguration();

            if (!reader.TryReadToFirstChildElement())
            {
                return configuration;
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsAtElementWithName(nameof(TestRunConfiguration.Parallelise)))
                {
                    reader.TryReadBoolean(b => configuration.Parallelise = b);
                }
                else if (reader.IsAtElementWithName(nameof(TestRunConfiguration.TestConfiguration)))
                {
                    configuration.TestConfiguration = TestConfigurationReader.ReadXml(reader);
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
