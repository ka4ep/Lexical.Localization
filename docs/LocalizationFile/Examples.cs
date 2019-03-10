﻿using Lexical.Localization;
using Lexical.Localization.Utils;
using Lexical.Localization.Internal;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace docs
{
    public class ILocalizationFileReader2_Examples
    {
        public static void Main(string[] args)
        {
            {
                #region Snippet_1a
                // Use explicit reader instance
                IAsset asset = IniLocalizationReader.Instance.FileAsset("localization.ini");
                #endregion Snippet_1a
            }
            {
                #region Snippet_1b
                // Infer reader instance from file extension '.ini'
                IAsset asset = LocalizationReaderMap.Instance.FileAsset("localization.ini");
                #endregion Snippet_1b
            }

            {
                #region Snippet_2
                // Add reader of custom .ext format to the global collection of readers.
                LocalizationReaderMap.Instance["ext"] = new ExtFileFormat();
                #endregion Snippet_2
            }

        }
    }

    #region Snippet_3
    class ExtFileFormat : ILocalizationKeyLinesStreamReader
    {
        public string Extension => "ext";
        public IEnumerable<KeyValuePair<IAssetKey, string>> ReadKeyLines(Stream stream, IAssetKeyNamePolicy namePolicy = default)
        {
            IAssetKey key = Key.Create("Section", "MyClass").Append("Key", "HelloWorld").Append("Culture", "en");
            yield return new KeyValuePair<IAssetKey, string>(key, "Hello World!");
        }
    }
    #endregion Snippet_3

}
