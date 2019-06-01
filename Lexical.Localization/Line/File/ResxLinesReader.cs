// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Reader that reads Microsoft's binary ".resx" files as key,value lines.
    /// </summary>
    public class ResxLinesReader : ILineFileFormat, ILineStringStreamReader, ILineStringTextReader
    {
        private readonly static ResxLinesReader instance = new ResxLinesReader();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ResxLinesReader Default => instance;

        /// <summary>
        /// File extension, "resx" for default.
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public IStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// Create new .resx reader instance with default values.
        /// </summary>
        public ResxLinesReader() : this("resx", CSharpFormat.Default) { }

        /// <summary>
        /// Create new .resx reader.
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="valueParser"></param>
        public ResxLinesReader(string ext, IStringFormat valueParser)
        {
            this.Extension = ext;
            this.ValueParser = valueParser as IStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
        }

        /// <summary>
        /// Read resx content from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">unused</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> ReadStringLines(Stream stream, ILineFormat lineFormat = default) => ReadElement(XDocument.Load(stream).Root, lineFormat);

        /// <summary>
        /// Read resx content from <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">unused</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> ReadStringLines(TextReader text, ILineFormat lineFormat = default) => ReadElement(XDocument.Load(text).Root, lineFormat);

        /// <summary>
        /// Reads lines from xml element.
        /// </summary>
        /// <param name="element">parent element that contains data elements</param>
        /// <param name="lineFormat">unused</param>
        /// <returns>lines</returns>
        public IEnumerable<KeyValuePair<string, IString>> ReadElement(XElement element, ILineFormat lineFormat)
        {
            foreach (XElement dataNode in element.Elements("data"))
            {
                XAttribute name = dataNode.Attribute("name");
                string key = name?.Value;
                if (key == null) continue;
                foreach(XElement valueNode in dataNode.Elements("value"))
                {
                    foreach (XNode textNode in valueNode.Nodes())
                    {
                        if (textNode is XText text)
                        {
                            IString value = ValueParser.Parse(text?.Value);
                            if (value != null)
                                yield return new KeyValuePair<string, IString>(key, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read lines from xml element, update <paramref name="correspondence"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="lineFormat">unused</param>
        /// <param name="correspondence"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, IString>> ReadElement(XElement element, ILineFormat lineFormat, ResXCorrespondence correspondence)
        {
            List<KeyValuePair<string, IString>> result = new List<KeyValuePair<string, IString>>();

            foreach (XElement dataNode in element.Elements("data"))
            {
                XAttribute name = dataNode.Attribute("name");
                string key = name?.Value;
                if (key == null) continue;
                if (correspondence != null) correspondence.Nodes.Put(key, dataNode);
                foreach (XElement valueNode in dataNode.Elements("value"))
                {
                    foreach (XNode textNode in valueNode.Nodes())
                    {
                        if (textNode is XText text)
                        {
                            IString value = ValueParser.Parse(text?.Value);
                            if (value != null)
                            {
                                result.Add(new KeyValuePair<string, IString>(key, value));
                                if (correspondence != null) correspondence.Values[new KeyValuePair<string, IString>(key, value)] = dataNode;
                            }
                        }
                    
                    }
                }
            }

            return result;
        }

    }

}
