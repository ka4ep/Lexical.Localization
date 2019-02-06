# Ms.Configuration

Microsoft.Extensions.Configuration.**IConfiguration** is an abstraction for loading configurations
of various sources. Configurations are key-value pairs, just like language strings are, 
which makes *IConfiguration* usable for localization use.

**ConfigurationLocalizationAsset** is a class that adapts *IConfiguration* into an *IAsset*.
Note that the calling assembly must import NuGet **Microsoft.Extensions.Configuration.Abstractions** and
namespace **Lexical.Localization.Ms.Extensions.Configuration**.
# [Snippet](#tab/snippet-1)

```csharp
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
```
# [Full Code](#tab/full-1)

```csharp
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

```
***

<br/>
If asset is constructed with asset builder, then **ConfigurationBuilderLocalizationAssetSource** 
can be used to adapt IConfigurationBuilder to IAssetSource. 
When asset is being built, asset source asks the IConfigurationBuilder to construct its IConfiguration, which will then be adapted to IAsset.

There is an extension method **.AddConfigurationBuilder()** that adds IConfigurationBuilder it to IAssetBuilder.
# [Snippet](#tab/snippet-2)

```csharp
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
```
# [Full Code](#tab/full-2)

```csharp
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

```
***

# Links
* [Microsoft.Extensions.Configuration.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/Configuration/Config.Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Abstractions/))
 * [IConfiguration](https://github.com/aspnet/Extensions/blob/master/src/Configuration/Config.Abstractions/src/IConfiguration.cs)
 * [IConfigurationBuilder](https://github.com/aspnet/Extensions/blob/master/src/Configuration/Config.Abstractions/src/IConfigurationBuilder.cs)
* [Microsoft.Extensions.Configuration](https://github.com/aspnet/Extensions/tree/master/src/Configuration/Config/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.Configuration/))
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [ConfigurationLocalizationAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Configuration/ConfigurationLocalizationAsset.cs)
 * [ConfigurationBuilderLocalizationAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Configuration/ConfigurationBuilderLocalizationAssetSource.cs)
