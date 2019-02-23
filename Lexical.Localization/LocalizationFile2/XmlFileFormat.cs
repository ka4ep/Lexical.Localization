using Lexical.Localization.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Lexical.Localization.LocalizationFile2
{
    public class XmlFileFormat
    {
        public const string URN = "urn:lexical.fi:";
        private readonly static XmlFileFormat instance = new XmlFileFormat();
        public static XmlFileFormat Instance => instance;
        private Key.Parametrizer parametrizer = Key.Parametrizer.Default;

        public KeyTree ReadFile(string filename)
            => ReadTree(XDocument.Load(filename).Root);
        public KeyTree ReadStream(Stream stream)
            => ReadTree(XDocument.Load(stream).Root);
        public KeyTree ReadText(TextReader reader)
            => ReadTree(XDocument.Load(reader).Root);
        public KeyTree ReadString(string xmlDocument)
            => ReadTree(XDocument.Load(new StringReader(xmlDocument)).Root);
        public KeyTree ReadTree(XElement element)
            => ReadElement(element, new KeyTree(Key.Parametrizer.Default.TryCreatePart(null, "root", "") as Key));

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
                key = parametrizer.CreatePart(key, parameterName, parameterValue) as Key;

                if (element.HasAttributes)
                    foreach (XAttribute attribute in element.Attributes())
                    {
                        if (string.IsNullOrEmpty(attribute.Name.NamespaceName))
                            key = parametrizer.CreatePart(key, attribute.Name.LocalName, attribute.Value) as Key;
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

    }

    public class XmlAsset : LocalizationAsset
    {
        public XmlAsset(string filename, bool reloadIfModified) : base()
        {
            KeyTree tree = XmlFileFormat.Instance.ReadFile(filename);
            IEnumerable<KeyValuePair<Key, string>> keyValues = tree.ToKeyValues(skipRoot: false).ToArray();
            AddKeySource(keyValues, filename);
            Load();
        }
    }

}
