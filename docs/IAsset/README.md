# IAsset
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

<details>
<summary>Table of IAsset interfaces. (<u>Click to expand</u>)</summary>

<table>
    <thead> 
    <tr> 
       <th>Name</th> 
       <th>Description</th> 
    </tr>
    </thead>
    <tbody>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAsset.cs">IAsset</a></td>
       <td>Root interface.</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetResourceProvider.cs">IAssetResourceProvider</a></td>
       <td>Provides culture specific binary resources, such as icons and sounds</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/LocalizationAsset/ILocalizationStringProvider.cs">ILocalizationStringProvider</a></td>
       <td>Provides culture specific language strings</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/LocalizationAsset/ILocalizationAssetCultureCapabilities.cs">ILocalizationAssetCultureCapabilities</a></td>
       <td>Enumerates available cultures</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs">IAssetCache</a></td>
       <td>Cache feature</td>
    </tr>
    <tr>
       <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs">IAssetCachePart</a></td>
       <td>Handles cache for specific interface</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetComposition.cs">IAssetComposition</a></td>
       <td>Composition of IAsset components</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetProvider.cs">IAssetProvider</a></td>
       <td>Loads IAssets from files as needed</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoader.cs">IAssetLoader</a></td>
       <td>Loads IAssets from files as needed, and is configurable</td>
    </tr>
    <tr>
       <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetLoaderPart.cs">IAssetLoaderPart</a></td>
       <td>Loader object</td>
    </tr>
    <tr>
       <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/IAssetLoaderPartResourceManager.cs">IAssetLoaderPartResourceManager</a></td>
       <td>Loader object for .resources files</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetReloadable.cs">IAssetReloadable</a></td>
       <td>Interface to reload content and clear caches.</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetBuilder.cs">IAssetBuilder</a></td>
       <td>Configurable to construct new IAsset instances</td>
    </tr>
    <tr>
       <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetSource.cs">IAssetSource</a></td>
       <td>Used with IAssetBuilder for Dependency Injection (DI) cases</td>
    </tr>
    </tbody>
</table>

</details>



<details>
  <summary>Table of IAsset implementing classes. (<u>Click to expand</u>)</summary>
<table>
<thead>
    <tr>
    <th>Name</th>
    <th>Description</th>
    </tr>
</thead>
<tbody>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/LocalizationAssetFunc.cs">LocalizationAssetFunc</a></td>
      <td>Calls delegate Func&lt;IAsset&gt; to provide IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/LocalizationStringDictionary.cs">LocalizationStringDictionary</a></td>
      <td>Adapts Dictionary&lt;string, string&gt; to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/LocalizationStringsFunc.cs">LocalizationStringsFunc</a></td>
      <td>Adapts Func&lt;IAssetKey, string&gt; to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/ResourceManagerAsset.cs">ResourceManagerAsset</a></td>
      <td>Adapts ResourceManager to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Configuration/ConfigurationLocalizationAsset.cs">ConfigurationLocalizationAsset</a></td>
      <td>Adapts IConfiguration to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/StringLocalizerAsset.cs">StringLocalizerAsset</a></td>
      <td>Adapts IStringLocalizer to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/StringLocalizerFactoryAsset.cs">StringLocalizerFactoryAsset</a></td>
      <td>Adapts IStringLocalizerFactory to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetBuilder.cs">AssetBuilder</a></td>
      <td>Instantiates new IAsset</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetSource.cs">AssetSource</a></td>
      <td>Adds asset as component when IAssetBuilder builds a new asset</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Configuration/ConfigurationBuilderLocalizationAssetSource.cs">ConfigurationBuilderLocalizationAssetSource</a></td>
      <td>Adapts IConfigurationBuilder to IAssetSource</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/ResourceManagerStringLocalizerAssetSource.cs">ResourceManagerStringLocalizerAssetSource</a></td>
      <td>Adapts location of .resources file to IAssetSource</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCache.cs">AssetCache</a></td>
      <td>Requests are cached</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCachePartResources.cs">AssetCachePartResources</a></td>
      <td>A part that adds feature to cache resource requests.</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/AssetCachePartStrings.cs">AssetCachePartStrings</a></td>
      <td>A part that adds feature to cache string requests.</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/AssetCachePartCultures.cs">AssetCachePartCultures</a></td>
      <td>A part that adds feature to cache culture enumeration requests.</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetComposition.cs">AssetComposition</a></td>
      <td>Composes IAsset composites into an unifying IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetResourceDictionary.cs">AssetResourceDictionary</a></td>
      <td>Converts Dictionary&lt;string, byte[]&gt; to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoader.cs">AssetLoader</a></td>
      <td>Loads assets as needed</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoaderPartFileResources.cs">AssetLoaderPartFileResources</a></td>
      <td>Part that loads binary assets from local files</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs">AssetLoaderPartFileProviderResources</a></td>
      <td>Part that loads binary assets using IFileProvider interface</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetLoader/AssetLoaderPartEmbeddedResources.cs">AssetLoaderPartEmbeddedResources</a></td>
      <td>Part that loads binary assets from embedded files</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartEmbeddedResourceManager.cs">AssetLoaderPartEmbeddedResourceManager</a></td>
      <td>Part that loads string assets from embedded .resources files</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartEmbeddedStrings.cs">AssetLoaderPartEmbeddedStrings</a></td>
      <td>Part that loads string assets from embedded text files</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartFileResourceManager.cs">AssetLoaderPartFileResourceManager</a></td>
      <td>Part that loads binary assets .resources files</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAssetLoader/AssetLoaderPartFileStrings.cs">AssetLoaderPartFileStrings</a></td>
      <td>Part that loads string assets from text files</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs">AssetLoaderPartFileProviderStrings</a></td>
      <td>Part that loads string assets from IFileProvider</td>
    </tr>
</tbody>
</table>

</details>


