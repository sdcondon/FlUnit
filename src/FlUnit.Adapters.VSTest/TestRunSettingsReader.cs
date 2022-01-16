////using System.IO;
////using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Reader for <see cref="TestRunSettings"/> instances.
    /// </summary>
    public static class TestRunSettingsReader
    {
        /// <summary>
        /// Reads and returns a <see cref="TestRunSettings"/> instance from an XML string.
        /// </summary>
        /// <param name="xml">The XML to read.</param>
        /// <returns>A new <see cref="TestRunSettings"/> instance, or <see cref="TestRunSettings.Default"/> if the argument is null or empty.</returns>
        public static TestRunSettings ReadXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return TestRunSettings.Default;
            }

            ////using (var stringReader = new StringReader(xml))
            ////{
            ////    XmlReader reader = XmlReader.Create(stringReader, XmlRunSettingsUtilities.ReaderSettings);
            ////
            ////    ... blah
            ////}
            return TestRunSettings.Default;
        }
    }
}
