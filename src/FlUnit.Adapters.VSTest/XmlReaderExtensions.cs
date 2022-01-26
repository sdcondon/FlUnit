using System;
using System.Xml;

namespace FlUnit.Adapters
{
    /// <summary>
    /// Useful exension methods for the <see cref="XmlReader"/> class.
    /// </summary>
    internal static class XmlReaderExtensions
    {
        /// <summary>
        /// Attempts to read to the first child element of the current element.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <returns>
        /// True if a child element has been navigated to; otherwise false.
        /// <para/>
        /// NB: this method has side effects: On success, the reader's current node will have advanced to the child element. On failure, it will be:
        /// <list type="bullet">
        /// <item>Unchanged if the current node is not an element, or is a self-closing element</item>
        /// <item>The end element of the current element if it is not self-closing, but contains no sub-elements.</item>
        /// </list>
        /// </returns>
        public static bool TryReadToFirstChildElement(this XmlReader reader)
        {
            if (reader.NodeType != XmlNodeType.Element || reader.IsEmptyElement)
            {
                return false;
            }

            while (!reader.EOF // possibly uneeded - since it would indicate malformed XML, which the XmlReader class handles by throwing..
                && reader.Read()
                && reader.NodeType != XmlNodeType.Element
                && reader.NodeType != XmlNodeType.EndElement)
            {
            }

            return reader.NodeType == XmlNodeType.Element;
        }

        /// <summary>
        /// Determines if the reader is currently positioned at an Element-type node with a specific name (by default case-insensitive).
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="name">The element name.</param>
        /// <param name="stringComparison">The string comparison to use.</param>
        /// <returns>True if the reader is currently positioned at an Element-type node with the specified name, otherwise false.</returns>
        public static bool IsAtElementWithName(this XmlReader reader, string name, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return reader.NodeType == XmlNodeType.Element && reader.Name.Equals(name, stringComparison);
        }

        /// <summary>
        /// Tries to read a boolean value from the inner XML of the current node.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="setter">The action to invoke with the read value, if successful.</param>
        /// <returns>True if a value has been successfully read, otherwise false.</returns>
        public static bool TryReadBoolean(this XmlReader reader, Action<bool> setter)
        {
            if (bool.TryParse(reader.ReadInnerXml(), out bool result))
            {
                setter(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to read an enum value from the inner XML of the current node.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="setter">The action to invoke with the read value, if successful.</param>
        /// <returns>True if a value has been successfully read, otherwise false.</returns>
        public static bool TryReadEnum<T>(this XmlReader reader, Action<T> setter)
            where T : struct
        {
            if (Enum.TryParse(reader.ReadInnerXml(), out T result))
            {
                setter(result);
                return true;
            }

            return false;
        }
    }
}
