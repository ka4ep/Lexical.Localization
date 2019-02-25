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
    public class XmlFileFormat : ILocalizationFileFormat, ILocalizationTreeStreamReader
    {
        public const string URN = "urn:lexical.fi:";
        private readonly static XmlFileFormat instance = new XmlFileFormat();
        public static XmlFileFormat Instance => instance;

        public string Extension => "xml";

        public KeyTree ReadFile(string filename) => ReadTree(XDocument.Load(filename).Root);
        public KeyTree ReadStream(Stream stream) => ReadTree(XDocument.Load(stream).Root);
        public KeyTree ReadText(TextReader reader)=> ReadTree(XDocument.Load(reader).Root);
        public KeyTree ReadString(string xmlDocument) => ReadTree(XDocument.Load(new StringReader(xmlDocument)).Root);
        public KeyTree ReadTree(XElement element) => ReadElement(element, new KeyTree(Key.Root));
        public IKeyTree ReadTree(Stream stream, IAssetKeyNamePolicy namePolicy) => ReadElement(XDocument.Load(stream).Root, new KeyTree(Key.Root));

        /// <summary>
        /// Reads <paramref name="element"/>, and adds as a subnode to <paramref name="parent"/> node.
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
        
    }

    public class XmlFileAsset : LocalizationAsset
    {
        public XmlFileAsset(string filename) : base()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IKeyTree keyTree = XmlFileFormat.Instance.ReadTree(stream, null);
                var lines = keyTree.ToLines(true).ToArray();
                AddKeySource(lines, filename);
                Load();
            }
        }

        public XmlFileAsset(Stream stream) : base()
        {
            IKeyTree keyTree = XmlFileFormat.Instance.ReadTree(stream, null);
            var lines = keyTree.ToLines(true).ToArray();
            AddKeySource(lines);
            Load();
        }
    }


}
