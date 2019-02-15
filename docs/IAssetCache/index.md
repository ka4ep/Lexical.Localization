# Asset Cache
**AssetCache**
is used for caching the requests in cases where asset implementation needs better performance.
Asset cache works as a decorator layer that forwards requests to its *source* and then stores the results. 

Asset cache needs to be populated with [IAssetCacheParts](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs) which each handle caching for a specific interface.
[!code-csharp[Snippet](Example_0.cs#Snippet)]

<br/>
There are extension methods for convenience.
[!code-csharp[Snippet](Example_1.cs#Snippet)]

<br/>
And then there is one extension method **CreateCache()** that wraps the asset into a cache and adds the default cache parts. 
[!code-csharp[Snippet](Example_2.cs#Snippet)]

<p/>
<details>
  <summary><b>IAssetCache</b> is the interface for cache implementatations. It is a composition of cache parts. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetCache.cs#interfaces)]
</details>

# Caching options
**AssetCacheOptions** carries a key-value map of caching parameters.
[!code-csharp[Snippet](Example_3.cs#Snippet)]

<br/>
Table of Asset cache option's keys

| Key      | Method  | Default | Description |
|----------|:--------|:--------|:------------|
| CloneKeys | .SetCloneKeys(bool) | true | Should cache create clones of keys, or should it use the keys that come from requests in its cache structures. |
| CacheStreams | .SetCacheStreams(bool) | true | Should IAssetResourceProvider#OpenStream requests be cached. |
| MaxResourceCount | .SetMaxResourceCount(int) | 2147483647 | Maximum number of resources to cache. |
| MaxResourceSize | .SetMaxResourceSize(int) | 4096 | Maximum size of a resource. |
| MaxResourceTotalSize | .SetMaxResourceTotalSize(int) | 1048576 | Maximum total number of bytes to reserve for all cached resources. |

<br/>
> [!NOTE]
> It is implementation specific whether option is supported or not. Some cache options may not be used.

# Clearing Cache
**IAsset.Reload()** clears caches and reloads assets from their configured sources.

[!code-csharp[Snippet](Example_4.cs#Snippet)]

# Links
* [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetCache)
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetCache](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetCache](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetCache.cs)
 * [AssetCachePartResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCachePartResources.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/)) 
 * [AssetCachePartStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/AssetCachePartStrings.cs)
 * [AssetCachePartCultures](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/AssetCachePartCultures.cs)
