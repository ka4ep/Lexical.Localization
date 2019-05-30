// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
using Lexical.Localization.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Lexical.Localization
{
    /// <summary>
    /// Class that reads .xml localization files.
    /// </summary>
    public class XmlLinesReader : ILineFileFormat, ILineTreeStreamReader, ILineTreeTextReader
    {
        /// <summary>
        /// Namespace
        /// </summary>
        public static readonly XNamespace NsDefault = "urn:lexical.fi";

        /// <summary>
        /// XName for Line element
        /// </summary>
        public static readonly XName NameLine = NsDefault + "Line";

        /// <summary>
        /// XName for document root
        /// </summary>
        public static readonly XName NameRoot = NsDefault + "Localization";

        /// <summary>
        /// URN 
        /// </summary>
        public const string URN_ = "urn:lexical.fi:";

        private readonly static XmlLinesReader instance = new XmlLinesReader();

        /// <summary>
        /// Default xml reader instance
        /// </summary>
        public static XmlLinesReader Instance => instance;

        /// <summary>
        /// File extension, default ".xml"
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public IStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// Xml reader settings
        /// </summary>
        protected XmlReaderSettings xmlReaderSettings;

        /// <summary>
        /// Create new xml reader
        /// </summary>
        public XmlLinesReader() : this("xml", CSharpFormat.Instance, default) { }

        /// <summary>
        /// Create new xml reader
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="valueParser"></param>
        /// <param name="xmlReaderSettings"></param>
        public XmlLinesReader(string extension, IStringFormat valueParser, XmlReaderSettings xmlReaderSettings = default)
        {
            this.Extension = extension;
            this.ValueParser = valueParser as IStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
            this.xmlReaderSettings = xmlReaderSettings ?? CreateXmlReaderSettings();
        }

        /// <summary>
        /// Read key tree from <paramref name="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="lineFormat">uses parameter info</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(XElement element, ILineFormat lineFormat = default)
            => ReadElement(element, new LineTree(), null, lineFormat.GetParameterInfos());

        /// <summary>
        /// Read key tree from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">uses parameter info</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(Stream stream, ILineFormat lineFormat = default)
            => ReadElement(Load(stream).Root, new LineTree(), null, lineFormat.GetParameterInfos());

        /// <summary>
        /// Read key tree from <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">uses parameter info</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(TextReader text, ILineFormat lineFormat = default)
            => ReadElement(Load(text).Root, new LineTree(), null, lineFormat.GetParameterInfos());

        /// <summary>
        /// Create default reader settings.
        /// </summary>
        /// <returns></returns>
        protected virtual XmlReaderSettings CreateXmlReaderSettings()
            => new XmlReaderSettings
            {
                CheckCharacters = false,
                CloseInput = false,
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreComments = false,
                Async = false,
                IgnoreWhitespace = false
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

        /// <summary>
        /// Reads <paramref name="element"/>, and adds as a subnode to <paramref name="parent"/> node.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        /// <param name="correspondenceContext">(optional) Correspondence to write element-tree mappings</param>
        /// <param name="parameterInfos"></param>
        /// <returns>parent</returns>
        public ILineTree ReadElement(XElement element, ILineTree parent, XmlCorrespondence correspondenceContext, IParameterInfos parameterInfos)
        {
            ILine key = ReadKey(element, parameterInfos);

            if (key != null)
            {
                ILineTree node = parent.CreateChild();
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
                            ILine lineValue = new LineHint(null, null, "Value", trimmedXmlValue);
                            node.Values.Add(lineValue);

                            if (correspondenceContext != null)
                                correspondenceContext.Values[new LineTreeValue(node, lineValue, node.Values.Count - 1)] = text;
                        }
                    }
                }

                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, node, correspondenceContext, parameterInfos);
                }
            }
            else
            {
                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, parent, correspondenceContext, parameterInfos);
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
            for (; startIx < len; startIx++)
            {
                char ch = value[startIx];
                if (ch != 10 && ch != 13 && ch != 32 && ch != 11) break;
            }
            int endIx = len - 1;
            for (; endIx >= startIx; endIx--)
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
        /// <param name="parameterInfos"></param>
        /// <returns>key or null</returns>
        public ILine ReadKey(XElement element, IParameterInfos parameterInfos)
        {
            ILine result;
            // <line type="MyClass" type="something" key="something">
            if (element.Name == NameLine)
            {
                result = null;
            }
            // <type:MyClass>
            else if (element.Name.NamespaceName != null && element.Name.NamespaceName.StartsWith(URN_))
            {
                string parameterName = element.Name.NamespaceName.Substring(URN_.Length);
                string parameterValue = element.Name.LocalName;
                result = Append(parameterInfos, null, parameterName, parameterValue);
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
                        result = Append(parameterInfos, result, parameterName, parameterValue);
                    }
                }
            }

            return result;
        }

        ILine Append(IParameterInfos parameterInfos, ILine prev, string parameterName, string parameterValue)
        {
            IParameterInfo info;
            if (parameterInfos.TryGetValue(parameterName, out info) && info != null)
            {
                if (info.InterfaceType == typeof(ILineHint)) return new LineHint(null, prev, parameterName, parameterValue);
                if (info.InterfaceType == typeof(ILineCanonicalKey)) return new LineKey.Canonical(null, prev, parameterName, parameterValue);
                if (info.InterfaceType == typeof(ILineNonCanonicalKey)) return new LineKey.NonCanonical(null, prev, parameterName, parameterValue);
            }
            return new LineParameter(null, prev, parameterName, parameterValue);
        }

        /// <summary>
        /// Parser that extracts name from occurance index "Key_2" -> "Key", "2".
        /// </summary>
        static Regex occuranceIndexParser = new Regex("^(?<name>.*)_(?<index>\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);


        /// <summary>
        /// List all children of <paramref name="parent"/> with a readable <see cref="ILine"/>.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameterInfos"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<ILine, XElement>> ListChildrenWithKeys(XElement parent, IParameterInfos parameterInfos)
            => parent.Elements().Select(e => new KeyValuePair<ILine, XElement>(ReadKey(e, parameterInfos), e)).Where(line => line.Key != null);

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

    }

}
