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

        /// <summary>
        /// Global singleton instance.
        /// </summary>
        public static IReadOnlyDictionary<string, ILineFileFormat> Default => instance;

    }
}
