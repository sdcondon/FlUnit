////using System.IO;
////using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Reader for <see cref="TestRunConfiguration"/> instances.
    /// </summary>
    public static class TestRunConfigurationReader
    {
        /// <summary>
        /// Reads and returns a <see cref="TestRunConfiguration"/> instance from an XML string.
        /// </summary>
        /// <param name="xml">The XML to read.</param>
        /// <returns>A new <see cref="TestRunConfiguration"/> instance, or <see cref="TestRunConfiguration.Default"/> if the argument is null or empty.</returns>
        public static TestRunConfiguration ReadXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return TestRunConfiguration.Default;
            }

            ////using (var stringReader = new StringReader(xml))
            ////{
            ////    XmlReader reader = XmlReader.Create(stringReader, XmlRunSettingsUtilities.ReaderSettings);
            ////
            ////    ... blah
            ////}
            return TestRunConfiguration.Default;
        }
    }
}
