using Lexical.Asset;
using Lexical.Asset.Ms.Extensions;
using Lexical.Localization.Ms.Extensions;
using Lexical.Localization;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Linq;

namespace docs
{
    public class Ms_FileProvider_Examples
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            // Create builder
            IAssetLoaderPart[] parts = new AssetLoaderPartBuilder()
                .FileProvider(fileProvider)
                .FilePattern("{location/}{culture/}{section/}{key}{.ext}")
                .MatchParameters("ext", "location")
                .Resource()
                .Build().ToArray();

            // Create asset
            IAsset asset = new AssetLoader(parts);
            #endregion Snippet
        }
    }

}
