using System.Reflection;
using Lexical.Asset;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace docs
{
    public class Ms_Configuration_Example1
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // Create configuration builder
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            // Create IFileProvider
            IFileProvider embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());

            // Add file provider
            configurationBuilder.SetFileProvider(embedded);

            // Add embedded .xml file
            configurationBuilder.AddXmlFile("localization.xml");

            // Build configuration
            IConfiguration configuration = configurationBuilder.Build();

            // Choose name policy that assumes that the configuration's first element is culture
            IAssetKeyNamePolicy namePolicy = ConfigurationLocalizationAsset.CULTURE_ROOT;

            // Adapt to asset
            IAsset asset = new ConfigurationLocalizationAsset(configuration, namePolicy);
            #endregion Snippet
        }
    }

}
