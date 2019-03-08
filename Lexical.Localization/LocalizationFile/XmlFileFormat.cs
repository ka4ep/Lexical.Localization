// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Lexical.Localization
{
    public class XmlFileFormat : ILocalizationFileFormat, ILocalizationKeyTreeStreamReader, ILocalizationKeyTreeTextReader
    {
        public static readonly XNamespace NsDefault = "urn:lexical.fi";
        public static readonly XName NameLine = NsDefault + "Line";
        public static readonly XName NameRoot = NsDefault + "Localization";
        public const string URN_ = "urn:lexical.fi:";

        private readonly static XmlFileFormat instance = new XmlFileFormat("xml");
        public static XmlFileFormat Instance => instance;
        public string Extension { get; protected set; }

        public XmlFileFormat() : this("xml") { }

        public XmlFileFormat(string extension)
        {
            this.Extension = extension;
        }

        public IKeyTree ReadKeyTree(XElement element, IAssetKeyNamePolicy namePolicy = default) 
            => ReadElement(element, new KeyTree(Key.Root), null);

        public IKeyTree ReadKeyTree(Stream stream, IAssetKeyNamePolicy namePolicy = default) 
            => ReadElement(XDocument.Load(stream).Root, new KeyTree(Key.Root), null);

        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default) 
            => ReadElement(XDocument.Load(text).Root, new KeyTree(Key.Root), null);

        /// <summary>
        /// Reads <paramref name="element"/>, and adds as a subnode to <paramref name="parent"/> node.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        /// <param name="correspondenceContext">(optional) Correspondence context</param>
        /// <returns>parent</returns>
        public IKeyTree ReadElement(XElement element, IKeyTree parent, KeyTreeXmlCorrespondence correspondenceContext)
        {
            IAssetKey key = ReadKey(element);

            if (key != null)
            {
                IKeyTree node = parent.GetOrCreate(key);

                if (correspondenceContext != null)
                    correspondenceContext.Nodes.Put(node, element);

                foreach (XNode nn in element.Nodes())
                {
                    if (nn is XText text)
                    {
                        string trimmedXmlValue = text?.Value?.Trim();
                        if (!string.IsNullOrEmpty(trimmedXmlValue))
                        {
                            node.Values.Add(trimmedXmlValue);

                            if (correspondenceContext!=null)
                                correspondenceContext.Values.Put(new KeyTreeValue { keyTree = node, valueIndex = node.Values.Count - 1, value = trimmedXmlValue }, text);
                        }
                    }
                }

                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, node, correspondenceContext);
                }
            }
            else
            {
                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, parent, correspondenceContext);
                }
            }

            return parent;
        }

        /// <summary>
        /// Read key from <paramref name="element"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>key or null</returns>
        public IAssetKey ReadKey(XElement element)
        {
            Key key;
            // <line type="MyClass" type="something" key="something">
            if (element.Name == NameLine)
            {
                key = null;
            }
            // <type:MyClass>
            else if (element.Name.NamespaceName != null && element.Name.NamespaceName.StartsWith(URN_))
            {
                string parameterName = element.Name.NamespaceName.Substring(URN_.Length);
                string parameterValue = element.Name.LocalName;
                key = Key.Create(null, parameterName, parameterValue);
            }
            else return null;

            // Read attributes
            if (element.HasAttributes)
            {
                foreach (XAttribute attribute in element.Attributes())
                {
                    if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                    {
                        string parameterName = attribute.Name.LocalName;
                        string parameterValue = attribute.Value;
                        Match m = occuranceIndexParser.Match(parameterName);
                        Group g_name = m.Groups["name"];
                        if (!m.Success || !g_name.Success) continue;
                        parameterName = g_name.Value;
                        // Append parameter
                        key = Key.Create(key, parameterName, parameterValue);
                    }
                }
            }

            return key;
        }

        /// <summary>
        /// Parser that extracts name from occurance index "Key_2" -> "Key", "2".
        /// </summary>
        static Regex occuranceIndexParser = new Regex("(?<name>.*)(_(?<index>\\d+))?$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);


        /// <summary>
        /// List all children of <paramref name="parent"/> with a readable <see cref="IAssetKey"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<IAssetKey, XElement>> ListChildrenWithKeys(XElement parent)
            => parent.Elements().Select(e => new KeyValuePair<IAssetKey, XElement>(ReadKey(e), e)).Where(line => line.Key != null);

    }

}
