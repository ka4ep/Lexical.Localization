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
using System.Xml.Linq;

namespace Lexical.Localization
{
    public class XmlFileFormat : ILocalizationFileFormat, ILocalizationKeyTreeStreamReader, ILocalizationKeyTreeTextReader
    {
        public const string URN = "urn:lexical.fi:";
        private readonly static XmlFileFormat instance = new XmlFileFormat();
        public static XmlFileFormat Instance => instance;
        public string Extension => "xml";

        public IKeyTree ReadKeyTree(XElement element, IAssetKeyNamePolicy namePolicy = default) => ReadElement(element, new KeyTree(Key.Root));
        public IKeyTree ReadKeyTree(Stream stream, IAssetKeyNamePolicy namePolicy = default) => ReadElement(XDocument.Load(stream).Root, new KeyTree(Key.Root));
        public IKeyTree ReadKeyTree(TextReader text, IAssetKeyNamePolicy namePolicy = default) => ReadElement(XDocument.Load(text).Root, new KeyTree(Key.Root));

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

}
