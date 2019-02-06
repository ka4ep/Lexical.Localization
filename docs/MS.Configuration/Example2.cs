using System.Reflection;
using Lexical.Asset;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace docs
{
    public class Ms_Configuration_Example2
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create file source
            IFileProvider embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            // Create configuration builder
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetFileProvider(embedded)
                .AddIniFile("localization.ini");

            // Create asset builder
            IAssetBuilder assetBuilder = new AssetBuilder();

            // Choose name policy that assumes that the configuration's first element is culture
            IAssetKeyNamePolicy namePolicy = ConfigurationLocalizationAsset.CULTURE_ROOT;

            // Use extension method that adds IConfigurationBuilder as IAssetSource
            assetBuilder.AddConfigurationBuilder(configurationBuilder, namePolicy);

            // Build asset
            IAsset asset = assetBuilder.Build();
            #endregion Snippet
        }
    }

}
