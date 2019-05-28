# Asset Cache
**AssetCache**
is used for caching the requests in cases where asset implementation needs better performance.
Asset cache works as a decorator layer that forwards requests to its *source* and then stores the results. 

Asset cache needs to be populated with [IAssetCacheParts](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs) which each handle caching for a specific interface.

```csharp
// Create asset
var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
IAsset asset = new LocalizationAsset(source, LineFormat.Parameters);

// Create cache
IAssetCache asset_cached = new AssetCache(asset);
// Adds feature to cache IAssetResourceProvider specific requests
asset_cached.Add(new AssetCachePartResources(asset_cached.Source, asset_cached.Options));
// Adds feature to cache ILocalizationStringProvider specific requests
asset_cached.Add(new AssetCachePartStrings(asset_cached.Source, asset_cached.Options));
// Adds feature to cache ILocalizationAssetCultureCapabilities specific requests
asset_cached.Add(new AssetCachePartCultures(asset_cached.Source, asset_cached.Options));

// Assign the cached asset
LineRoot.Global.Asset = asset_cached;
```

<br/>
There are extension methods for convenience.

```csharp
// Create cache decorator
IAssetCache asset_cached = new AssetCache(asset).AddResourceCache().AddStringsCache().AddCulturesCache();
```

<br/>
And then there is one extension method **CreateCache()** that wraps the asset into a cache and adds the default cache parts. 

```csharp
// Decorate with cache
IAssetCache asset_cached = asset.CreateCache();
```

<p/>
<details>
  <summary><b>IAssetCache</b> is the interface for cache implementatations. It is a composition of cache parts. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Asset cache is decorator that caches requests of source object.
/// 
/// The interface is used as signal for extension methods.
/// </summary>
public interface IAssetCache : IAssetComposition
{
    /// <summary>
    /// Source asset.
    /// </summary>
    IAsset Source { get; }

    /// <summary>
    /// Cache options.
    /// </summary>
    AssetCacheOptions Options { get; }
}

/// <summary>
/// Part that addresses a feature (an interface) to cache.
/// </summary>
public interface IAssetCachePart : IAsset
{
}
```
</details>

# Caching options
**AssetCacheOptions** carries a key-value map of caching parameters.

```csharp
// Create asset
var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
IAsset asset = new LocalizationAsset(source, LineFormat.Parameters);

// Create cache
IAssetCache asset_cached = asset.CreateCache();

// Configure options
asset_cached.Options.SetCloneKeys(true);
asset_cached.Options.SetCacheStreams(true);
asset_cached.Options.SetMaxResourceCount(1024);
asset_cached.Options.SetMaxResourceSize(1024 * 1024);
asset_cached.Options.SetMaxResourceTotalSize(1024 * 1024 * 1024);

// Assign the asset with cache decoration
LineRoot.Global.Asset = asset_cached;
```

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


```csharp
// Create asset
var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
IAsset asset = new LocalizationAsset(source, LineFormat.Parameters);

// Cache it
asset = asset.CreateCache();

// Issue a request which will be cached.
ILine key = new LineRoot().Key("hello");
IFormatString value = asset.GetString( key.Culture("en") ).GetValue();
Console.WriteLine(value);

// Clear cache
asset.Reload();
```

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
