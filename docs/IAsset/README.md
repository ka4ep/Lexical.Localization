# Introduction
Asset is a class that manages localization sources.
Sources are typically files, embedded resources, and plain code.

```csharp
// Language string source
Dictionary<string, string> src = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
// Create Asset
IAsset asset = new LocalizationStringDictionary(src);
```

IAsset is the root interface for assets. It serves as a signal that the implementing class has further asset features.
There are more specific interfaces such as **ILocalizationStringProvider** and **IAssetResourceProvider** which 
retrieve language strings and binary resources.

Asset interfaces are not called directly but used instead by calling extension methods of IAsset.

```csharp
// Create key
IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
// Resolve string - Call to LocalizationAssetExtensions.GetString()
string str = asset.GetString(key);
```

# Asset Interfaces
Table of IAsset interfaces

| Name     | Description |
|----------|:------------|
| [IAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAsset.cs)   | Root interface. |
| [IAssetResourceProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetResourceProvider.cs)    | Provides culture specific binary resources, such as icons and sounds |
| [ILocalizationStringProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/LocalizationAsset/ILocalizationStringProvider.cs)    | Provides culture specific language strings |
| [ILocalizationAssetCultureCapabilities](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/LocalizationAsset/ILocalizationAssetCultureCapabilities.cs) | Enumerates available cultures |
| [IAssetCache](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetCache.cs)    | Cache feature |
| &nbsp;&nbsp;&nbsp;&nbsp;[IAssetCachePart](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetCache.cs) | Handles cache for specific interface |
| [IAssetComposition](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetComposition.cs)    | Composition of IAsset components  |
| [IAssetProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetProvider.cs)    | Loads IAssets from files as needed  |
| [IAssetLoader](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetLoader.cs)    | Loads IAssets from files as needed, and is configurable |
| &nbsp;&nbsp;&nbsp;&nbsp;[IAssetLoaderPart](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetLoaderPart.cs) | Loader object |
| &nbsp;&nbsp;&nbsp;&nbsp;[IAssetLoaderPartResourceManager](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAssetLoader/IAssetLoaderPartResourceManager.cs) | Loader object for .resources files |
| [IAssetReloadable](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetReloadable.cs)    | Interface to reload content and clear caches. |
| [IAssetBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetBuilder.cs)    | Configurable to construct new IAsset instances |
| &nbsp;&nbsp;&nbsp;&nbsp;[IAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetSource.cs)    | Used with IAssetBuilder for Dependency Injection (DI) cases |

# Asset Classes
Table of IAsset implementing classes

| Name | Description |
|----------|:-------|
| [LocalizationAssetFunc](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAsset/LocalizationAssetFunc.cs) | Calls delegate Func&lt;IAsset&gt; to provide IAsset |
| [LocalizationStringDictionary](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAsset/LocalizationStringDictionary.cs) | Adapts Dictionary&lt;string, string&gt; to IAsset |
| [LocalizationStringsFunc](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAsset/LocalizationStringsFunc.cs) | Adapts Func&lt;IAssetKey, string&gt; to IAsset |
| [ResourceManagerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAsset/ResourceManagerAsset.cs) | Adapts ResourceManager to IAsset |
| [ConfigurationLocalizationAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Configuration/ConfigurationLocalizationAsset.cs) | Adapts IConfiguration to IAsset |
| [StringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Localization/StringLocalizerAsset.cs) | Adapts IStringLocalizer to IAsset |
| [StringLocalizerFactoryAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Localization/StringLocalizerFactoryAsset.cs) | Adapts IStringLocalizerFactory to IAsset |
| [AssetBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Asset/AssetBuilder.cs) | Instantiates new IAsset |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Asset/AssetSource.cs) | Adds asset as component when IAssetBuilder builds a new asset |
| &nbsp;&nbsp;&nbsp;&nbsp;[ConfigurationBuilderLocalizationAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Configuration/ConfigurationBuilderLocalizationAssetSource.cs) | Adapts IConfigurationBuilder to IAssetSource |
| &nbsp;&nbsp;&nbsp;&nbsp;[ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/Localization/ResourceManagerStringLocalizerAssetSource.cs) | Adapts location of .resources file to IAssetSource |
| [AssetCache](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Asset/AssetCache.cs) | Requests are cached |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetCachePartResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Asset/AssetCachePartResources.cs) | A part that adds feature to cache resource requests. |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetCachePartStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAsset/AssetCachePartStrings.cs) | A part that adds feature to cache string requests. |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetCachePartCultures](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAsset/AssetCachePartCultures.cs) | A part that adds feature to cache culture enumeration requests. |
| [AssetComposition](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Asset/AssetComposition.cs) | Composes IAsset composites into an unifying IAsset |
| [AssetResourceDictionary](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Asset/AssetResourceDictionary.cs) | Converts Dictionary&lt;string, byte[]&gt; to IAsset |
| [AssetLoader](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/AssetLoader/AssetLoader.cs) | Loads assets as needed |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartFileResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/AssetLoader/AssetLoaderPartFileResources.cs) | Part that loads binary assets from local files |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartFileProviderResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs) | Part that loads binary assets using IFileProvider interface |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartEmbeddedResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/AssetLoader/AssetLoaderPartEmbeddedResources.cs) | Part that loads binary assets from embedded files |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartEmbeddedResourceManager](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAssetLoader/AssetLoaderPartEmbeddedResourceManager.cs) | Part that loads string assets from embedded .resources files |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartEmbeddedStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAssetLoader/AssetLoaderPartEmbeddedStrings.cs) | Part that loads string assets from embedded text files |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartFileResourceManager](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAssetLoader/AssetLoaderPartFileResourceManager.cs) | Part that loads binary assets .resources files |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartFileStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/LocalizationAssetLoader/AssetLoaderPartFileStrings.cs) | Part that loads string assets from text files |
| &nbsp;&nbsp;&nbsp;&nbsp;[AssetLoaderPartFileProviderStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs) | Part that loads string assets from IFileProvider |
