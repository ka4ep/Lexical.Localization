// --------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.2.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using Lexical.Localization.Internal;
using Lexical.Localization.StringFormat;
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
    public class ResourcesLineReader : ILineFileFormat, IUnformedLineStreamReader
    {
        private readonly static ResourcesLineReader instance = new ResourcesLineReader();

        /// <summary>
        /// Default instance
        /// </summary>
        public static ResourcesLineReader Default => instance;

        /// <summary>
        /// File extension
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Value string parser.
        /// </summary>
        public IStringFormatParser ValueParser { get; protected set; }

        /// <summary>
        /// Create reader
        /// </summary>
        public ResourcesLineReader() : this("resources", CSharpFormat.Default) { }

        /// <summary>
        /// Create reader
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="valueParser"></param>
        public ResourcesLineReader(string ext, IStringFormat valueParser)
        {
            this.Extension = ext;
            this.ValueParser = valueParser as IStringFormatParser ?? throw new ArgumentNullException(nameof(valueParser));
        }

        /// <summary>
        /// Read string lines
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lineFormat">unused</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, IString>> ReadUnformedLines(Stream stream, ILineFormat lineFormat = default)
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
                        IString formatString = ValueParser.Parse(value);
                        yield return new KeyValuePair<string, IString>(key, formatString);
                    }
                }
            }
        }

    }

}
