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
    /// <summary>
    /// Reader that reads Microsoft's binary ".resources" files as key,value lines.
    /// </summary>
    public class LocalizationResourcesReader : ILocalizationFileFormat, ILocalizationStringLinesStreamReader
    {
        private readonly static LocalizationResourcesReader instance = new LocalizationResourcesReader();
        public static LocalizationResourcesReader Instance => instance;

        public string Extension { get; protected set; }

        public LocalizationResourcesReader() : this("resources") { }
        public LocalizationResourcesReader(string ext)
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
