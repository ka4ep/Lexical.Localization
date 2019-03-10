// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
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
    public class ResXLocalizationReader : ILocalizationFileFormat, ILocalizationStringLinesStreamReader, ILocalizationStringLinesTextReader
    {
        private readonly static ResXLocalizationReader instance = new ResXLocalizationReader();
        public static ResXLocalizationReader Instance => instance;
        public string Extension { get; protected set; }

        public ResXLocalizationReader() : this("resx") { }
        public ResXLocalizationReader(string ext) {
            this.Extension = ext;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadStringLines(Stream stream, IAssetKeyNamePolicy namePolicy = default) => ReadElement(XDocument.Load(stream).Root, namePolicy);
        public IEnumerable<KeyValuePair<string, string>> ReadStringLines(TextReader text, IAssetKeyNamePolicy namePolicy = default) => ReadElement(XDocument.Load(text).Root, namePolicy);

        /// <summary>
        /// Reads lines from xml element.
        /// </summary>
        /// <param name="element">parent element that contains data elements</param>
        /// <returns>lines</returns>
        public IEnumerable<KeyValuePair<string, string>> ReadElement(XElement element, IAssetKeyNamePolicy namePolicy)
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
                            string value = text?.Value;
                            if (value != null)
                                yield return new KeyValuePair<string, string>(key, value);
                        }
                    }
                }
            }
        }

        public List<KeyValuePair<string, string>> ReadElement(XElement element, IAssetKeyNamePolicy namePolicy, StringLineXmlCorrespondence correspondence)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

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
                            string value = text?.Value;
                            if (value != null)
                            {
                                result.Add(new KeyValuePair<string, string>(key, value));
                                if (correspondence != null) correspondence.Values.Put(new Pair<string, string>(key, value), text);
                            }
                        }
                    
                    }
                }
            }

            return result;
        }

    }

}
