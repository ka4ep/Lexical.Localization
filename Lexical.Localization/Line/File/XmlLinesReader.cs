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

        private readonly static XmlLinesReader non_resolving = new XmlLinesReader("xml", LineAppender.NonResolving);
        private readonly static XmlLinesReader resolving = new XmlLinesReader("xml", LineAppender.Default);

        /// <summary>
        /// .json file lines reader that does not resolve parameters into instantances.
        /// 
        /// Used when handling localization files as texts, not for localization
        /// </summary>
        public static XmlLinesReader NonResolving => non_resolving;

        /// <summary>
        /// .json file lines reader that resolves parameters into instantances.
        /// 
        /// <list type="bullet">
        ///     <item>Parameter "Culture" is created as <see cref="ILineCulture"/></item>
        ///     <item>Parameter "String" is created as to <see cref="ILineString"/></item>
        ///     <item>Parameter "StringFormat" is created as to <see cref="ILineStringFormat"/></item>
        ///     <item>Parameter "Functions" is created as to <see cref="ILineFunctions"/></item>
        ///     <item>Parameter "PluralRules" is created as to <see cref="ILinePluralRules"/></item>
        ///     <item>Parameter "FormatProvider" is created as to <see cref="ILineFormatProvider"/></item>
        /// </list>
        /// 
        /// Used when reading localization files for localization purposes.
        /// </summary>
        public static XmlLinesReader Default => resolving;

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

        /// <summary>
        /// File extension, default ".xml"
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Line factory that instantiates lines.
        /// </summary>
        public ILineFactory LineFactory { get; protected set; }

        /// <summary>
        /// Resolver that was extracted from LineFactory.
        /// </summary>
        protected IResolver resolver;

        /// <summary>
        /// Xml reader settings
        /// </summary>
        protected XmlReaderSettings xmlReaderSettings;

        /// <summary>
        /// Create new xml reader
        /// </summary>
        public XmlLinesReader() : this("xml", LineAppender.Default, default) { }

        /// <summary>
        /// Create new xml reader
        /// </summary>
        /// <param name="lineFactory"></param>
        public XmlLinesReader(ILineFactory lineFactory) : this("xml", lineFactory, default) { }

        /// <summary>
        /// Create new xml reader
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="lineFactory"></param>
        /// <param name="xmlReaderSettings"></param>
        public XmlLinesReader(string extension, ILineFactory lineFactory, XmlReaderSettings xmlReaderSettings = default)
        {
            this.Extension = extension;
            this.xmlReaderSettings = xmlReaderSettings ?? CreateXmlReaderSettings();
            this.LineFactory = lineFactory ?? throw new ArgumentNullException(nameof(lineFactory));
            lineFactory.TryGetResolver(out resolver);
        }

        /// <summary>
        /// Read key tree from <paramref name="element"/>
        /// </summary>
        /// <param name="element"></param>
        /// <param name="lineFormat">uses parameter info</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(XElement element, ILineFormat lineFormat = default)
        {
            ILineFactory _lineFactory;
            if (!lineFormat.TryGetLineFactory(out _lineFactory)) _lineFactory = LineFactory;
            return ReadElement(element, new LineTree(), null, _lineFactory);
        }

        /// <summary>
        /// Read key tree from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">uses parameter info</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(Stream stream, ILineFormat lineFormat = default)
        {
            ILineFactory _lineFactory;
            if (!lineFormat.TryGetLineFactory(out _lineFactory)) _lineFactory = LineFactory;
            return ReadElement(Load(stream).Root, new LineTree(), null, _lineFactory);
        }

        /// <summary>
        /// Read key tree from <paramref name="text"/>.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="lineFormat">uses parameter info</param>
        /// <returns></returns>
        public ILineTree ReadLineTree(TextReader text, ILineFormat lineFormat = default)
        {
            ILineFactory _lineFactory;
            if (!lineFormat.TryGetLineFactory(out _lineFactory)) _lineFactory = LineFactory;
            return ReadElement(Load(text).Root, new LineTree(), null, _lineFactory);
        }

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
        /// <param name="_lineFactory">line factory to use.</param>
        /// <returns>parent</returns>
        public ILineTree ReadElement(XElement element, ILineTree parent, XmlCorrespondence correspondenceContext, ILineFactory _lineFactory)
        {
            ILine key = ReadKey(element, parent, _lineFactory);

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
                            ILine lineValue = new LineHint(null, null, "String", trimmedXmlValue);
                            node.Values.Add(lineValue);

                            if (correspondenceContext != null)
                                correspondenceContext.Values[new LineTreeValue(node, lineValue, node.Values.Count - 1)] = text;
                        }
                    }
                }

                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, node, correspondenceContext, _lineFactory);
                }
            }
            else
            {
                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, parent, correspondenceContext, _lineFactory);
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
        /// <param name="parent"></param>
        /// <param name="_lineFactory"></param>
        /// <returns>key or null</returns>
        public ILine ReadKey(XElement element, ILineTree parent, ILineFactory _lineFactory)
        {
            ILine result = null;
            // <line type="MyClass" type="something" key="something">
            if (element.Name == NameLine || element.Name == NameRoot)
            {
                result = null;
            }
            // <type:MyClass>
            else if (element.Name.NamespaceName != null && element.Name.NamespaceName.StartsWith(URN_))
            {
                string parameterName = element.Name.NamespaceName.Substring(URN_.Length);
                string parameterValue = element.Name.LocalName;
                result = Append(parent, _lineFactory, result, parameterName, parameterValue);
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
                        if (parameterName.StartsWith("xml")) continue;

                        Match m = occuranceIndexParser.Match(parameterName);
                        Group g_name = m.Groups["name"];
                        if (m.Success && g_name.Success) parameterName = g_name.Value;
                        // Append parameter
                        result = Append(parent, _lineFactory, result, parameterName, parameterValue);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Append value to <paramref name="prev"/>>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="_lineFactory"></param>
        /// <param name="prev">(optional)</param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        ILine Append(ILineTree parent, ILineFactory _lineFactory, ILine prev, string parameterName, string parameterValue)
        {
            if (parameterName == "String")
            {
                IStringFormat stringFormat;
                if (parent.TryGetStringFormat(resolver, out stringFormat))
                {
                    IString valueString = stringFormat.Parse(parameterValue);
                    return LineFactory.Create<ILineString, IString>(prev, valueString);
                }
                else
                {
                    return LineFactory.Create<ILineHint, string, string>(prev, "String", parameterValue);
                }
            } else
            {
                return LineFactory.Create<ILineParameter, string, string>(prev, parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Parser that extracts name from occurance index "Key_2" -> "Key", "2".
        /// </summary>
        static Regex occuranceIndexParser = new Regex("^(?<name>.*)_(?<index>\\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);


        /// <summary>
        /// List all children of <paramref name="element"/> with a readable <see cref="ILine"/>.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="node"></param>
        /// <param name="_lineFactory"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<ILine, XElement>> ListChildrenWithKeys(XElement element, ILineTree node, ILineFactory _lineFactory)
            => element.Elements().Select(e => new KeyValuePair<ILine, XElement>(ReadKey(e, node, _lineFactory), e)).Where(line => line.Key != null);

    }

}
