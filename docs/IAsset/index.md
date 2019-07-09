# IAsset
Asset is a class that manages localization sources.
Sources are typically files, embedded resources, and plain code.
[!code-csharp[Snippet](IAsset_Examples.cs#Snippet_1)]

IAsset is the root interface for assets. It serves as a signal that the implementing class has further asset features.
There are more specific interfaces such as **IStringAsset** and **IResourceAsset** which 
retrieve language strings and binary resources.

Asset interfaces are not called directly but used instead by calling extension methods of IAsset.
[!code-csharp[Snippet](IAsset_Examples.cs#Snippet_2)]


# Asset Composition

**AssetComposition** is the default class. It unifies a group of assets into one asset, typically so that they can be assigned to ILineRoot.
[!code-csharp[Snippet](IAssetComposition_Example.cs#Snippet)]

<details>
  <summary><b>IAssetComposition</b> is the interface for classes that composes IAsset components. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetComposition.cs#doc)]
</details>


# Asset Builder
**AssetBuilder** is a factory class that constructs new instances of IAsset. 
Asset builder is populated with IAssetSources which participate to the build process. 

This example shows how to create asset builder, add asset sources, and then to build an asset.
[!code-csharp[Snippet](IAssetBuilder_Example_0.cs#Snippet)]

There are extension methods for convenience.
[!code-csharp[Snippet](IAssetBuilder_Example_1.cs#Snippet)]

Asset builders and asset sources are used with Dependency Injection. 
Asset sources are added to IServiceCollection to participate in constructing in new assets.
Asset builder makes the new asset when requested by ServiceProvider.
The calling assembly must have nuget dependency to [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/).

[!code-csharp[Snippet](IAssetBuilder_Example_2.cs#Snippet)]

Extension method **AddLexicalLocalization()** adds IAsset, ILineRoot, ICultureProvider and IAssetBuilder services to IServiceCollection.
[!code-csharp[Snippet](IAssetBuilder_Example_3.cs#Snippet)]

<details>
  <summary><b>IAssetBuilder</b> is the interface for factory class(es) that instantiate IAssets. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetBuilder.cs#doc)]
</details>
<details>
  <summary><b>IAssetSource</b> is the interface for sources that contribute asset(s) to the built result. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetSource.cs#doc)]
</details>



# Asset Cache
**AssetCache**
is used for caching the requests in cases where asset implementation needs better performance.
Asset cache works as a decorator layer that forwards requests to its *source* and then stores the results. 

Asset cache needs to be populated with [IAssetCacheParts](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs) which each handle caching for a specific interface.
[!code-csharp[Snippet](IAssetCache_Example_0.cs#Snippet)]

<br/>
There are extension methods for convenience.
[!code-csharp[Snippet](IAssetCache_Example_1.cs#Snippet)]

<br/>
And then there is one extension method **CreateCache()** that wraps the asset into a cache and adds the default cache parts. 
[!code-csharp[Snippet](IAssetCache_Example_2.cs#Snippet)]

<p/>
<details>
  <summary><b>IAssetCache</b> is the interface for cache implementatations. It is a composition of cache parts. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetCache.cs#doc)]
</details>

## Caching options
**AssetCacheOptions** carries a key-value map of caching parameters.
[!code-csharp[Snippet](IAssetCache_Example_3.cs#Snippet)]

<br/>
Table of Asset cache option's keys

| Key      | Method  | Default | Description |
|----------|:--------|:--------|:------------|
| CloneKeys | .SetCloneKeys(bool) | true | Should cache create clones of keys, or should it use the keys that come from requests in its cache structures. |
| CacheStreams | .SetCacheStreams(bool) | true | Should IResourceAsset#OpenStream requests be cached. |
| MaxResourceCount | .SetMaxResourceCount(int) | 2147483647 | Maximum number of resources to cache. |
| MaxResourceSize | .SetMaxResourceSize(int) | 4096 | Maximum size of a resource. |
| MaxResourceTotalSize | .SetMaxResourceTotalSize(int) | 1048576 | Maximum total number of bytes to reserve for all cached resources. |

<br/>
> [!NOTE]
> It is implementation specific whether option is supported or not. Some cache options may not be used.

## Clearing Cache
**IAsset.Reload()** clears caches and reloads assets from their configured sources.

[!code-csharp[Snippet](IAssetCache_Example_4.cs#Snippet)]

# Links
* [IAssetComposition](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetComposition.cs)
* [AssetComposition](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetComposition.cs)
* [IAssetBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetBuilder.cs)
* [IAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetSource.cs)
* [AssetBuilder](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetBuilder.cs)
* [AssetSource](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetSource.cs) Passes IAsset to to builder.
* [AssetCacheSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCache.cs) Adds cache to the built asset. 
* [ResourceManagerStringLocalizerAsset](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/ResourceManagerStringLocalizerAsset.cs) Adapts location of .resources file to IAsset.
* [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/ResourceManagerStringLocalizerAssetSource.cs) Adapts location of .resources file to IAssetSource.
* [IAssetCache](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs)
* [AssetCache](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetCache.cs)
