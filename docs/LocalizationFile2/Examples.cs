using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization.LocalizationFile;
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
                using (FileStream fs = new FileStream("localization.ini", FileMode.Open))
                {
                    // Get .ext file format
                    ILocalizationFileStreamReader fileFormat = LocalizationFileFormatMap.Singleton.TryGet("ini") as ILocalizationFileStreamReader;
                    // Create reader
                    ILocalizationFileTokenizer textReader = fileFormat.OpenStream(fs, AssetKeyNameProvider.Default);
                    // Convert to asset
                    IAsset asset = textReader.ToAssetAndClose();
                }
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                // Add reader of custom .ext format to the global collection of readers.
                LocalizationFileFormatMap.Singleton["ext"] = new ExtFileFormat2();
                #endregion Snippet_2
            }

        }
    }

    #region Snippet_3
    class ExtFileFormat2 : ILocalizationFileFormat, ILocalizationFileStreamReader
    {
        public string Extension 
            => "ext";
        public ILocalizationFileTokenizer OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = null)
            => new ExtReader2(stream, namePolicy);
    }

    class ExtReader2 : ILocalizationFileTokenizer
    {
        public IAssetKeyNamePolicy NamePolicy => throw new System.NotImplementedException();
        public ExtReader2(Stream stream, IAssetKeyNamePolicy namePolicy)
        {
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Token> Read()
        {
            throw new System.NotImplementedException();
        }
    }
    #endregion Snippet_3

}
