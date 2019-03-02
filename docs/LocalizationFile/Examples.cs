using Lexical.Localization;
using Lexical.Localization.Utils;
using Lexical.Localization.Internal;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace docs
{
    public class ILocalizationFileReader2_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                IAsset asset = IniFileFormat.Instance.CreateFileAsset("localization.ini");
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                // Add reader of custom .ext format to the global collection of readers.
                LocalizationFileFormatMap.Singleton["ext"] = new ExtFileFormat();
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
            IAssetKey key = Key.Create("section", "MyClass").Append("key", "HelloWorld").Append("culture", "en");
            yield return new KeyValuePair<IAssetKey, string>(key, "Hello World!");
        }
    }
    #endregion Snippet_3

}
