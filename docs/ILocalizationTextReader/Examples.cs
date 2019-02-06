using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization;
using Lexical.Localization.LocalizationFile;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace docs
{
    public class ILocalizationFileReader_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            {
                #region Snippet_1
                using (FileStream fs = new FileStream("localization.ext", FileMode.Open))
                {
                    // Get .ext file format
                    ILocalizationFileStreamReader fileFormat = LocalizationFileFormatMap.Singleton.TryGet("ext") as ILocalizationFileStreamReader;
                    // Create reader
                    ILocalizationFileReadable textReader = fileFormat.OpenStream(fs, AssetKeyNameProvider.Default);
                    // Convert to asset
                    IAsset asset = textReader.ToAssetAndClose();
                }
                #endregion Snippet_1
            }

            {
                #region Snippet_2
                // Add reader of .ext files (ExtReader) to the collection of readers.
                LocalizationFileFormatMap.Singleton["ext"] = new ExtFileFormat();
                #endregion Snippet_2
            }

        }
    }

    class ExtFileFormat : ILocalizationFileFormat, ILocalizationFileStreamReader
    {
        public string Extension 
            => "ext";
        public ILocalizationFileReadable OpenStream(Stream stream, IAssetKeyNamePolicy namePolicy = null)
            => new ExtReader(stream, namePolicy);
    }

    class ExtReader : ILocalizationFileReadable
    {
        public IAssetKeyNamePolicy NamePolicy => throw new System.NotImplementedException();
        public ExtReader(Stream stream, IAssetKeyNamePolicy namePolicy)
        {
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TextElement> Read()
        {
            throw new System.NotImplementedException();
        }
    }

}
