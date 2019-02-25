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
    public class XmlFileFormat : ILocalizationFileFormat, ILocalizationTreeStreamReader//, ILocalizationTreeStreamWriter
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
        public void WriteTree(IKeyTree tree, Stream stream, IAssetKeyNamePolicy namePolicy)
        {
            XDocument doc = new XDocument();
            doc.Root.Name = "localization";
            //WriteElement(tree, doc.Root, doc.Root);
            doc.Save(stream);
        }

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
        
        /// <summary>
        /// Write or update contents of <paramref name="node"/> and its subtree.
        /// 
        /// </summary>
        /// <param name="node">node and subtree to write</param>
        /// <param name="parent">parent node under which <paramref name="node"/> is written to</param>
        /// <param name="root">Root where namespaces are written</param>
        /// <param name="touchedElements">(optional) if set, all nodes that were touched are added to this list</param>
        public void UpdateElement(IKeyTree node, XElement parent, XElement root, ICollection<XNode> touchedElements)
        {
            int ix = 0;
            XElement dst = new XElement("node");
            foreach(var parameter in node.Key.GetParameters())
            {
                if (String.IsNullOrEmpty(parameter.Key) || parameter.Key == "root") continue;
                if (ix++==0)
                {
                } else
                {

                }
            }
        }
    }

    public class XmlAsset : LocalizationAsset
    {
        public XmlAsset(string filename, bool reloadIfModified) : base()
        {
            AddKeySource(XmlFileFormat.Instance.FileSource(filename), filename);
            Load();
        }
    }


}
