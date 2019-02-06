# Ms.FileProvider
*IFileProvider* is an abstaction for the variance of file sources.

Language string files and other localization resources can be read from IFileProvider source.
Loading requires an asset loader and 
[loader part](../IAssetLoader/PartClasses/index.html#file-provider). Note that, the calling assembly must:
* import nuget **Microsoft.Extensions.FileProviders.Abstractions**
* import namespace **Lexical.Localization.Ms.Extensions** 
* import namespace **Lexical.Asset.Ms.Extensions**

# [Snippet](#tab/snippet-1)

```csharp
// File provider
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

// Create part that reads strings from ".ini" files.
IAssetLoaderPart part = new AssetLoaderPartFileProviderStrings(fileProvider, @"Assets\localization{-culture}.ini", AssetFileConstructors.Ini);

// Create loader
IAssetLoader assetLoader = new AssetLoader(part);
```
# [Full Code](#tab/full-1)

```csharp
using Lexical.Asset;
using Lexical.Asset.Ms.Extensions;
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace docs
{
    public class IAssetLoader_Example_8
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            #region Snippet
            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            // Create part that reads strings from ".ini" files.
            IAssetLoaderPart part = new AssetLoaderPartFileProviderStrings(fileProvider, @"Assets\localization{-culture}.ini", AssetFileConstructors.Ini);

            // Create loader
            IAssetLoader assetLoader = new AssetLoader(part);
            #endregion Snippet

            // Create key
            IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("de");
            // Issue request. Asset loader matches filename "Assets\localization-de.ini".
            assetLoader.GetString(key);
        }
    }

}

```
***

<br/>
Asset loader part can also be constructed with 
[Part builder](../IAssetLoader/PartBuilder/index.html#source-type).

# [Snippet](#tab/snippet-2)

```csharp
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
```
# [Full Code](#tab/full-2)

```csharp
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

```
***



# Links
* See
 * [Part Loader](../IAssetLoader/PartClasses/index.html#file-provider)
 * [Part Builder](../IAssetLoader/PartBuilder/index.html#source-type)
* [Microsoft.Extensions.FileProviders.Abstractions](https://github.com/aspnet/Extensions/tree/master/src/FileProviders/Abstractions/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Abstractions/))
 * [IFileProvider](https://github.com/aspnet/Extensions/blob/master/src/FileProviders/Abstractions/src/IFileProvider.cs)
* [Microsoft.Extensions.FileProviders.Physical](https://github.com/aspnet/Extensions/tree/master/src/FileProviders/Physical/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Physical/))
* [Microsoft.Extensions.FileProviders.Embedded](https://github.com/aspnet/Extensions/tree/master/src/FileProviders/Embedded/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Embedded/))
* [Microsoft.Extensions.FileProviders.Composite](https://github.com/aspnet/Extensions/tree/master/src/FileProviders/Composite/src) ([NuGet](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Composite/))
* [Lexical.Asset](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Asset) ([NuGet](https://www.nuget.org/packages/Lexical.Asset/))
 * [AssetLoaderPartBuilder](https://github.com/tagcode/Lexical.Asset/blob/master/Lexical.Asset/Ms.Extensions/FileProvider/AssetLoaderPartBuilder.cs)
 * [AssetLoaderPartFileProviderResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetLoaderPartBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartBuilder.cs)
 * [AssetLoaderPartFileProviderStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs)
