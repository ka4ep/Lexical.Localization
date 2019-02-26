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
    public class ResXFileFormat : ILocalizationFileFormat, ILocalizationStringLinesStreamReader, ILocalizationStringLinesTextReader
    {
        private readonly static ResXFileFormat instance = new ResXFileFormat();
        public static ResXFileFormat Instance => instance;
        public string Extension => "resx";

        public IEnumerable<KeyValuePair<string, string>> ReadStringLines(Stream stream, IAssetKeyNamePolicy namePolicy = default) => ReadElement(XDocument.Load(stream).Root, namePolicy);
        public IEnumerable<KeyValuePair<string, string>> ReadStringLines(TextReader text, IAssetKeyNamePolicy namePolicy = default) => ReadElement(XDocument.Load(text).Root, namePolicy);

        /// <summary>
        /// Reads lines from xml element.
        /// </summary>
        /// <param name="element">parent element that contains data elements</param>
        /// <returns>lines</returns>
        public IEnumerable<KeyValuePair<string, string>> ReadElement(XElement element, IAssetKeyNamePolicy namePolicy)
        {
            foreach(XElement dataNode in element.Elements("data"))
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
    }

    public class ResXFileAsset : LocalizationAsset
    {
        public ResXFileAsset(string filename, string policy) : this(filename, new AssetNamePattern(policy)) { }
        public ResXFileAsset(string filename, IAssetKeyNamePolicy policy) : base()
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var lines = ResXFileFormat.Instance.ReadKeyLines(stream, policy).ToArray();
                AddKeySource(lines, filename);
                Load();
            }
        }

        public ResXFileAsset(Stream stream, string policy) : this(stream, new AssetNamePattern(policy)) { }
        public ResXFileAsset(Stream stream, IAssetKeyNamePolicy policy) : base()
        {
            var lines = ResXFileFormat.Instance.ReadKeyLines(stream, policy).ToArray();
            AddKeySource(lines);
            Load();
        }
    }

}
