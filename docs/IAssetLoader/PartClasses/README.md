# Introduction
Parts are added to asset loader. They load IAsset instances on need basis.

```csharp
// Create part
IAssetLoaderPart part =
    new AssetLoaderPartFileStrings(
        filenamePattern: "localization{-culture}.ini",
        keyNamePattern: AssetKeyNameProvider.None_Colon_Colon)
    .AddPath(".");

// Create asset laoder
IAsset asset = new AssetLoader(part);

// Create key
IAssetKey key = new LocalizationRoot(asset).Section("controller").Key("hello").SetCulture("de");

// Issue request. Asset loader matches to filename "localization-de.ini", loads it,
// and then searches for string with key "controller:hello".
string str = key.ToString();
```

# File Strings
**AssetLoaderPartFileStrings** is part class that loads localization strings from local files.

For example, let's assume the application has following asset file structure. 
```none
Application.exe
Assets\
    localization.ini
	localization-sv.ini
	localization-fi.ini
	localization-de.ini
```

AssetLoaderPartFileStrings loads strings.

```csharp
// Create asset loader
IAssetLoader assetLoader = new AssetLoader();

// Create part that reads strings from ".ini" files.
IAssetLoaderPart part_1 = new AssetLoaderPartFileStrings("Assets/localization{-culture}.ini", "{anysection_0:}{anysection_1:}{anysection_n:}[key]").AddPath(".");

// Add part to loader
assetLoader.Add(part_1);

// Create key
IAssetKey key = new LocalizationRoot().Section("controller").Key("hello").SetCulture("de");

// Issue request. Asset loader matches to filename "Assets\localization-de.ini", loads it,
// and then searches for string with key "controller:hello".
string str = assetLoader.GetString(key);
```

# Embedded Strings
In similar way to loading assets from files, they can also be loaded from embedded resources. Let's assume your application has the following embedded resources.
```none
docs.Assets.localization.ini
docs.Assets.localization-sv.ini
docs.Assets.localization-fi.ini
docs.Assets.localization-de.ini
```

Asset loader is configured in the same manner, but with different class.

```csharp
// Create part that reads strings from embedded ".ini" files.
IAssetLoaderPart part = new AssetLoaderPartEmbeddedStrings(@"[assembly.]Assets.localization{-culture}.ini")
    .AddAssembly(Assembly.GetExecutingAssembly());

// Create asset loader
AssetLoader assetLoader = new AssetLoader( part );

// Create key
IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("de");

// Issue request. Asset loader matches culture "de" to filename and tries to load "namespace.Assets.localization-de.ini".
assetLoader.GetString(key);
```

# Binary resources
There are two types of localization assets: strings and binary resources.
Binary resources are graphics, icons, audio files, and other resource that are language and location specific.
The asset part classes that load binaries have slightly different naming, 
the is distinction in the end of the class names: **AssetLoaderPartFile*Strings*** loads strings and **AssetLoaderPartFile*Resource*** loads binaries.

**AssetLoaderPartFileResources** loads binary resources from files.

```csharp
// Create loader that can read ".png" files.
IAssetLoaderPart part = new AssetLoaderPartFileResources(@"Assets/icon{-key}{-culture}.png").AddPaths(".");

// Create asset loader
IAssetLoader assetLoader = new AssetLoader(part);

// Create key
IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
// Issue request. Asset loader matches to filename "Assets\icon-ok-de.png".
byte[] data = assetLoader.GetResource(key);
```

**AssetLoaderPartEmbeddedResources** loads binary resources from *embedded resources*.

```csharp
// Create loader that can read ".png" files.
IAssetLoaderPart part = new AssetLoaderPartEmbeddedResources("[Assembly.]Assets.icon{-key}{-culture}.png").AddAssembly(Assembly.GetExecutingAssembly());

// Create asset loader
IAssetLoader assetLoader = new AssetLoader(part);

// Create key
IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
// Issue request. Asset loader matches to filename "docs.Assets.icon-ok-de.png".
byte[] data = assetLoader.GetResource(key);
```

# File Provider
Third type of source is [IFileProvider](https://github.com/aspnet/Extensions/blob/master/src/FileProviders/Abstractions/src/IFileProvider.cs).
[AssetLoaderPartFileProviderStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs)
loads strings from file providers. The calling assembly must have nuget dependency to **[Microsoft.Extensions.FileProviders.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Abstractions/)**,
and import namespaces **Lexical.Localization.Ms.Extensions** and **Lexical.Localization.Ms.Extensions**.
# [Snippet](#tab/snippet-8)

```csharp
// File provider
IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

// Create part that reads strings from ".ini" files.
IAssetLoaderPart part = new AssetLoaderPartFileProviderStrings(fileProvider, @"Assets\localization{-culture}.ini", AssetFileConstructors.Ini);

// Create loader
IAssetLoader assetLoader = new AssetLoader(part);
```
# [Full Code](#tab/full-8)

```csharp
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


And [AssetLoaderPartFileProviderResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs)
loads binary resources.

# [Snippet](#tab/snippet-10)

```csharp
// Create part that reads strings from ".ini" files.
IAssetLoaderPart part = new AssetLoaderPartFileProviderResources(fileProvider, @"Assets\icon{-key}{-culture}.png");

// Create loader
IAssetLoader assetLoader = new AssetLoader(part);
```
# [Full Code](#tab/full-10)

```csharp
using Lexical.Localization;
using Lexical.Localization.Ms.Extensions;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace docs
{
    public class IAssetLoader_Example_10
    {
        // Rename to "Main", or run from Main.
        public static void Run(string[] args)
        {
            // File provider
            IFileProvider fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            #region Snippet
            // Create part that reads strings from ".ini" files.
            IAssetLoaderPart part = new AssetLoaderPartFileProviderResources(fileProvider, @"Assets\icon{-key}{-culture}.png");

            // Create loader
            IAssetLoader assetLoader = new AssetLoader(part);
            #endregion Snippet

            // Create key
            IAssetKey key = new LocalizationRoot().Key("ok").SetCulture("de");
            // Issue request. Asset loader matches to filename "Assets\icon-ok-de.png".
            byte[] data = assetLoader.GetResource(key);
        }
    }

}

```
***

# Options
[AssetLoaderPartOptions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoader.cs) is is a IDictionary&lt;string, object&gt;
that carries options for IAssetLoaderParts. 

<br/>
Table of asset part options.

| Key      | Method  | Description |
|----------|:--------|:------------|
| MatchParameters | .AddMatchParameters(string) | Determines which parameters of a key name pattern, such as "assembly" or "location" are to be matched against detected files. |
| Assemblies | .AddAssembly(Assembly) | Assemblies to search embedded resources from. |
| Paths | .AddPaths(IEnumerable&lt;string&gt;) | Folders to search files from. |

Asset parts are configured with options. This is needed if part builder is not used. 
What options are supported or needed is implementation specific.

```csharp
// Create part that searches for localization.ini files in every folder.
IAssetLoaderPart part = new AssetLoaderPartFileStrings("{location/}localization{-culture}.ini");
// Set root path
part.Options.AddPath(".");
// Add option to match "{location}" parameter to existing files.
part.Options.AddMatchParameter("location");
// Create asset loader
IAssetLoader assetLoader = new AssetLoader(part);
```

# Links
* [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetLoader/PartClasses)
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetLoaderPart](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPart.cs)
 * [IAssetLoaderPartOptions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPartOptions.cs)

Table of IAssetLoaderPart implementations

| Name | Source Type | File Type(s) | Description |
|----------|:-------|:-------|:-------|
| [AssetLoaderPartFileStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartFileStrings.cs) | file | strings | Loads string assets from text files |
| [AssetLoaderPartFileResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoaderPartFileResources.cs) | file | binary | Loads binary assets from local files |
| [AssetLoaderPartFileResourceManager](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartFileResourceManager.cs) | file | .resources/.resx | Loads binary assets .resources files |
| [AssetLoaderPartEmbeddedStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartEmbeddedStrings.cs) | embedded resources | strings | Loads string assets from embedded text files |
| [AssetLoaderPartEmbeddedResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoaderPartEmbeddedResources.cs) | embedded resources | binary | Loads binary assets from embedded files |
| [AssetLoaderPartEmbeddedResourceManager](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartEmbeddedResourceManager.cs) | embedded resource | .resources/.resx | Loads string assets from embedded .resources files |
| [AssetLoaderPartFileProviderResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs) | IFileProvider | binary | Loads binary assets using IFileProvider interface |
| [AssetLoaderPartFileProviderStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs) | IFileProvider | strings | Loads string assets from text files using IFileProvider interface |
