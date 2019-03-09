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

namespace Lexical.Localization
{
    public class ResourcesLocalizationReader : ILocalizationFileFormat, ILocalizationStringLinesStreamReader
    {
        private readonly static ResourcesLocalizationReader instance = new ResourcesLocalizationReader();
        public static ResourcesLocalizationReader Instance => instance;

        public string Extension { get; protected set; }

        public ResourcesLocalizationReader() : this("resources") { }
        public ResourcesLocalizationReader(string ext)
        {
            this.Extension = ext;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadStringLines(Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                IDictionaryEnumerator dict = reader.GetEnumerator();
                while (dict.MoveNext())
                {
                    string key = null, value = null;
                    try
                    {
                        if (dict.Key is string _key && dict.Value is string _value)
                        { key = _key; value = _value; }
                    }
                    catch (Exception e)
                    {
                        throw new LocalizationException("Failed to read .resources file", e);
                    }
                    if (key != null && value != null)
                        yield return new KeyValuePair<string, string>(key, value);
                }
            }
        }

    }

}
