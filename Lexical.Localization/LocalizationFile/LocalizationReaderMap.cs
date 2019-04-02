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
    public class LocalizationReaderMap : LocalizationFileFormatMap
    {
        static IReadOnlyDictionary<string, ILocalizationFileFormat> instance = 
            new LocalizationFileFormatMap(
                LocalizationXmlReader.Instance, 
                LocalizationResxReader.Instance, 
                LocalizationResourcesReader.Instance, 
                LocalizationJsonReader.Instance, 
                LocalizationIniReader.Instance);

        /// <summary>
        /// Global singleton instance.
        /// </summary>
        public static IReadOnlyDictionary<string, ILocalizationFileFormat> Instance => instance;

    }
}
