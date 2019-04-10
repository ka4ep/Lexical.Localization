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

        /// <summary>
        /// Default instance
        /// </summary>
        public static LocalizationResourcesReader Instance => instance;

        /// <summary>
        /// File extension
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public ILocalizationStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// Create reader
        /// </summary>
        public LocalizationResourcesReader() : this("resources", LexicalStringFormat.Instance) { }

        /// <summary>
        /// Create reader
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="valueParser"></param>
        public LocalizationResourcesReader(string ext, ILocalizationStringFormat valueParser)
        {
            this.Extension = ext;
            this.ValueParser = valueParser as ILocalizationStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
        }

        /// <summary>
        /// Read string lines
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="namePolicy"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IFormulationString>> ReadStringLines(Stream stream, IAssetKeyNamePolicy namePolicy = default)
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
                    {
                        IFormulationString formulationString = ValueParser.Parse(value);
                        yield return new KeyValuePair<string, IFormulationString>(key, formulationString);
                    }
                }
            }
        }

    }

}
