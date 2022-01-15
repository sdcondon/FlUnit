////using System.IO;
////using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Reader for <see cref="RunSettings"/> instances.
    /// </summary>
    public static class RunSettingsReader
    {
        /// <summary>
        /// Reads and create a new <see cref="RunSettings"/> instance from an XML string.
        /// </summary>
        /// <param name="xml">The XML to read.</param>
        /// <returns>A new <see cref="RunSettings"/> instance. If <see cref="xml"/> is null or empty, the default settings will be returned.</returns>
        public static RunSettings ReadXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return RunSettings.Default;
            }

            ////using (var stringReader = new StringReader(xml))
            ////{
            ////    XmlReader reader = XmlReader.Create(stringReader, XmlRunSettingsUtilities.ReaderSettings);
            ////
            ////    ... blah
            ////}
            return RunSettings.Default;
        }
    }
}
