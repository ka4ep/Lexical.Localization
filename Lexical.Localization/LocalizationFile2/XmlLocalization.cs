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

namespace Lexical.Localization.LocalizationFile2
{
    public class XmlLocalization
    {
        public const string URN = "urn:lexical.fi:";
        private readonly static XmlLocalization instance = new XmlLocalization();
        public static XmlLocalization Instance => instance;

        public KeyTree ReadFile(string filename)
            => ReadTree(XDocument.Load(filename).Root);
        public KeyTree ReadStream(Stream stream)
            => ReadTree(XDocument.Load(stream).Root);
        public KeyTree ReadText(TextReader reader)
            => ReadTree(XDocument.Load(reader).Root);
        public KeyTree ReadString(string xmlDocument)
            => ReadTree(XDocument.Load(new StringReader(xmlDocument)).Root);
        public KeyTree ReadTree(XElement element)
            => ReadElement(element, new KeyTree(Key.Root));

        /// <summary>
        /// Reads element, and adds as a subnode to parent node.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        /// <returns>parent</returns>
        public KeyTree ReadElement(XElement element, KeyTree parent)
        {            
            Key key = null;

            if (element.Name.NamespaceName != null && element.Name.NamespaceName.StartsWith(URN))
            {
                string parameterName = element.Name.NamespaceName.Substring(URN.Length);
                string parameterValue = element.Name.LocalName;
                key = Key.Create(key, parameterName, parameterValue);

                if (element.HasAttributes)
                    foreach (XAttribute attribute in element.Attributes())
                    {
                        if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                            key = Key.Create(key, attribute.Name.LocalName, attribute.Value);
                    }
            }

            if (key != null)
            {
                KeyTree node = parent.GetOrCreateChild(key);
                foreach (XNode nn in element.Nodes())
                {
                    if (nn is XText text) node.Values.Add(text.Value);
                }

                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, node);
                }
            }
            else
            {
                if (element.HasElements)
                {
                    foreach (XElement e in element.Elements())
                        ReadElement(e, parent);
                }
            }

            return parent;
        }

        public class Asset : LocalizationAsset
        {
            public Asset(string filename, bool reloadIfModified) : base()
            {
                KeyTree tree = XmlLocalization.Instance.ReadFile(filename);
                IEnumerable<KeyValuePair<IAssetKey, string>> keyValues = tree.ToKeyValues(skipRoot: false).ToArray();
                AddKeySource(keyValues, filename);
                Load();
            }
        }

        /// <summary>
        /// Xml-file source that reads file into memory every time <see cref="IEnumerator{T}"/> is acquired.
        /// </summary>
        public class FileSource : IEnumerable<KeyValuePair<Key, string>>
        {
            /// <summary>
            /// Xml file name
            /// </summary>
            public readonly string filename;

            public FileSource(string filename)
            {
                this.filename = filename ?? throw new ArgumentNullException(nameof(filename));
            }

            public IEnumerator<KeyValuePair<Key, string>> GetEnumerator()
                => ((IEnumerable<KeyValuePair<Key, string>>)XmlLocalization.Instance.ReadFile(filename).ToKeyValues(true)).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => XmlLocalization.Instance.ReadFile(filename).ToKeyValues(true).GetEnumerator();
        }

    }



}
