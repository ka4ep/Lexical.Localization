﻿//---------------------------------------------------------
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
                XmlLinesReader.Instance, 
                ResxLinesReader.Instance, 
                ResourcesLineReader.Instance, 
                JsonLinesReader.Instance, 
                IniLinesReader.Instance);

        /// <summary>
        /// Global singleton instance.
        /// </summary>
        public static IReadOnlyDictionary<string, ILineFileFormat> Instance => instance;

    }
}
