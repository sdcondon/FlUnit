using System.IO;
using System.Xml;

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

        /// <summary>
        /// Reads and returns a <see cref="TestRunConfiguration"/> instance from an XML string.
        /// </summary>
        /// <param name="xml">The XML to read.</param>
        /// <param name="elementName">
        /// The name of the element within the provided XML that contains the run configuration.
        /// NB: The first matching element will be used, not the shallowest (i.e. its found with a DFS, not a BFS).
        /// The shallowest would perhaps make more sense, but it'd be surprising if this ever actually caused a problem.
        /// </param>
        /// <returns>A new <see cref="TestRunConfiguration"/> instance, this will be a default instance if the argument is null or empty.</returns>
        public static TestRunConfiguration ReadFromXml(string xml, string elementName)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new TestRunConfiguration();
            }

            using (var stringReader = new StringReader(xml))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                while (!xmlReader.EOF && xmlReader.Read() && !xmlReader.IsAtElementWithName(elementName))
                {
                }

                return ReadFromXml(xmlReader);
            }
        }

        /// <summary>
        /// Reads and returns a <see cref="TestRunConfiguration"/> instance from an XML reader.
        /// </summary>
        /// <param name="reader">A reader positioned at the root element of the test run configuration.</param>
        /// <returns>A new <see cref="TestRunConfiguration"/> instance.</returns>
        public static TestRunConfiguration ReadFromXml(XmlReader reader)
        {
            TestRunConfiguration configuration = new TestRunConfiguration();

            if (reader.TryReadToFirstChildElement())
            {
                while (reader.NodeType != XmlNodeType.EndElement)
                {
                    if (reader.IsAtElementWithName(nameof(Parallelise)))
                    {
                        reader.TryReadBoolean(b => configuration.Parallelise = b);
                    }
                    else if (reader.IsAtElementWithName(nameof(TestConfiguration)))
                    {
                        configuration.TestConfiguration = TestConfiguration.ReadFromXml(reader);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
            }

            return configuration;
        }
    }
}
