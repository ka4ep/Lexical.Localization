//---------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           19.1.2019
// Url:            http://lexical.fi
// --------------------------------------------------------
using System.Collections.Generic;

namespace Lexical.Localization
{
    /// <summary>
    /// Collection of localization readers.
    /// </summary>
    public class LineReaderMap : LineFileFormatMap
    {
        static IReadOnlyDictionary<string, ILineFileFormat> instance = 
            new LineFileFormatMap(
                XmlLinesReader.Default, 
                ResxLinesReader.Default, 
                ResourcesLineReader.Default, 
                JsonLinesReader.Default, 
                IniLinesReader.Default);

        static IReadOnlyDictionary<string, ILineFileFormat> nonResolving =
            new LineFileFormatMap(
                XmlLinesReader.NonResolving,
                ResxLinesReader.Default,
                ResourcesLineReader.Default,
                JsonLinesReader.NonResolving,
                IniLinesReader.NonResolving);

        /// <summary>
        /// Global singleton instance. 
        /// 
        /// File formats in this map resolves parameters into respective classes.
        /// 
        /// </summary>
        public static IReadOnlyDictionary<string, ILineFileFormat> Default => instance;

        /// <summary>
        /// Global singleton instance.
        /// 
        /// File formats in this map do not resolve parameters, but returns them as strings.
        /// </summary>
        public static IReadOnlyDictionary<string, ILineFileFormat> NonResolving => nonResolving;

    }
}
