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
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Class that reads .xml localization files.
    /// </summary>
    public class LocalizationXmlReader : ILocalizationFileFormat, ILocalizationKeyTreeStreamReader, ILocalizationKeyTreeTextReader
    {
        public static readonly XNamespace NsDefault = "urn:lexical.fi";
        public static readonly XName NameLine = NsDefault + "Line";
        public static readonly XName NameRoot = NsDefault + "Localization";
        public const string URN_ = "urn:lexical.fi:";

        private readonly static LocalizationXmlReader instance = new LocalizationXmlReader("xml");

        public static LocalizationXmlReader Instance => instance;
        public string Extension { get; protected set; }

        XmlReaderSettings xmlReaderSettings;

        public LocalizationXmlReader() : this("xml", default) { }

        public LocalizationXmlReader(string extension, XmlReaderSettings xmlReaderSettings = default)
        {
            this.Extension = extension;
            this.xmlReaderSettings = xmlReaderSettings ?? CreateXmlReaderSettings();
        }

        public IKeyTree ReadKeyTree(XElement element, IAssetKeyNamePolicy namePolicy = default)
            => ReadElement(element, new KeyTree(Key.Root), null);

        public IKeyTree ReadKeyTree(Stream stream, IAssetKeyNamePolicy namePolicy = default)
            => ReadElement(Load(stream).Root, new KeyTree(Key.Root), null);

        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default)
            => ReadElement(Load(text).Root, new KeyTree(Key.Root), null);

        protected virtual XmlReaderSettings CreateXmlReaderSettings()
            => new XmlReaderSettings
            {
                CheckCharacters = false,
                CloseInput = false,
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreComments = false,
                Async = false,
                IgnoreWhitespace = false,
                ValidationType = ValidationType.Auto
            };

        /// <summary>
        /// Load xml document
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        protected virtual XDocument Load(Stream stream, LoadOptions loadOptions = LoadOptions.None)
        {
            //XmlReader xmlReader = new TrimmerXmlReader(stream);
            //return XDocument.Load(xmlReader, loadOptions);

            using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                return XDocument.Load(xmlReader, loadOptions);
        }

        /// <summary>
        /// Load xml document
        /// </summary>
        /// <param name="textReader"></param>
        /// <param name="loadOptions"></param>
        /// <returns></returns>
        protected virtual XDocument Load(TextReader textReader, LoadOptions loadOptions = LoadOptions.None)
        {
            //XmlReader xmlReader = new TrimmerXmlReader(textReader);
            //return XDocument.Load(xmlReader, loadOptions);
            using (XmlReader xmlReader = XmlReader.Create(textReader, xmlReaderSettings))
                return XDocument.Load(xmlReader, loadOptions);
        }
        /*
        public class TrimmerXmlReader : XmlTextReader
        {
            public TrimmerXmlReader(Stream input) : base(input) { }
            public TrimmerXmlReader(TextReader input) : base(input) { }
            public TrimmerXmlReader(string url) : base(url) { }
            public TrimmerXmlReader(Stream input, XmlNameTable nt) : base(input, nt) { }
            public TrimmerXmlReader(TextReader input, XmlNameTable nt) : base(input, nt) { }
            public TrimmerXmlReader(string url, Stream input) : base(url, input) { }
            public TrimmerXmlReader(string url, TextReader input) : base(url, input) { }
            public TrimmerXmlReader(string url, XmlNameTable nt) : base(url, nt) { }
            public TrimmerXmlReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context) { }
            public TrimmerXmlReader(string url, Stream input, XmlNameTable nt) : base(url, input, nt) { }
            public TrimmerXmlReader(string url, TextReader input, XmlNameTable nt) : base(url, input, nt) { }
            public TrimmerXmlReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context) : base(xmlFragment, fragType, context) { }
            protected TrimmerXmlReader() { }
            protected TrimmerXmlReader(XmlNameTable nt) : base(nt) { }
            public override bool Read()
            {
                bool ok = base.Read();
                return ok;
            }
        }*/

        /// <summary>
        /// Reads <paramref name="element"/>, and adds as a subnode to <paramref name="parent"/> node.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        /// <param name="correspondenceContext">(optional) Correspondence to write element-tree mappings</param>
        /// <returns>parent</returns>
        public IKeyTree ReadElement(XElement element, IKeyTree parent, XmlCorrespondence correspondenceContext)
        {
            IAssetKey key = ReadKey(element);

            if (key != null)
            {
                IKeyTree node = parent.CreateChild();
                node.Key = key;

                if (correspondenceContext != null)
                    correspondenceContext.Nodes.Put(node, element);

                foreach (XNode nn in element.Nodes())
                {
                    if (nn is XText text)
                    {
                        string trimmedXmlValue = Trim(text?.Value);
                        if (!string.IsNullOrEmpty(trimmedXmlValue))
                        {
                            node.Values.Add(trimmedXmlValue);

                            if (correspondenceContext != null)
                                correspondenceContext.Values[new KeyTreeValue(node, trimmedXmlValue, node.Values.Count - 1)] = text;
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
        /// Trim white space
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string Trim(string value)
        {
            if (value == null) return null;
            int len = value.Length;
            if (len == 0) return value;
            int startIx = 0;
            for(; startIx<len; startIx++)
            {
                char ch = value[startIx];
                if (ch != 10 && ch != 13 && ch != 32 && ch != 11) break;
            }
            int endIx = len - 1;
            for(;endIx>=startIx;endIx--)
            {
                char ch = value[endIx];
                if (ch != 10 && ch != 13 && ch != 32 && ch != 11) break;
            }
            if (startIx == 0 && endIx == len - 1) return value;
            if (startIx == endIx + 1) return "";
            return value.Substring(startIx, endIx - startIx + 1);
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
                        if (m.Success && g_name.Success) parameterName = g_name.Value;
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
        static Regex occuranceIndexParser = new Regex("^(?<name>.*)_(?<index>\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);


        /// <summary>
        /// List all children of <paramref name="parent"/> with a readable <see cref="IAssetKey"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<IAssetKey, XElement>> ListChildrenWithKeys(XElement parent)
            => parent.Elements().Select(e => new KeyValuePair<IAssetKey, XElement>(ReadKey(e), e)).Where(line => line.Key != null);

    }

}
