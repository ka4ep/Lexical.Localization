# IAsset
Asset is a class that manages localization sources.
Sources are typically files, embedded resources, and plain code.

```csharp
// Language string source
Dictionary<string, string> src = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
// Create Asset
IAsset asset = new StringAsset(src, LineParameterPrinter.Default);
```

IAsset is the root interface for assets. It serves as a signal that the implementing class has further asset features.
There are more specific interfaces such as **IStringAsset** and **IResourceAsset** which 
retrieve language strings and binary resources.

Asset interfaces are not called directly but used instead by calling extension methods of IAsset.

```csharp
// Create key
ILine key = new LineRoot().Key("hello").Culture("en");
// Resolve string - Call to StringAssetExtensions.GetString()
IString str = asset.GetLine(key).GetString();
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
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IResourceAsset.cs">IResourceAsset</a></td>
       <td>Provides culture specific binary resources, such as icons and sounds</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/StringAsset/IStringAsset.cs">IStringAsset</a></td>
       <td>Provides culture specific language strings</td>
    </tr>
    <tr>
       <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/StringAsset/IAssetCultureEnumerable.cs">IAssetCultureEnumerable</a></td>
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
       <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAssetLoader/IAssetLoaderPartResourceManager.cs">IAssetLoaderPartResourceManager</a></td>
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
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/AssetFunc.cs">AssetFunc</a></td>
      <td>Calls delegate Func&lt;IAsset&gt; to provide IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringAsset.cs">StringAsset</a></td>
      <td>Adapts Dictionary&lt;string, string&gt; to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringAssetFunc.cs">StringAssetFunc</a></td>
      <td>Adapts Func&lt;ILine, string&gt; to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/ResourceManagerAsset.cs">ResourceManagerAsset</a></td>
      <td>Adapts ResourceManager to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringLocalizerAsset.cs">StringLocalizerAsset</a></td>
      <td>Adapts IStringLocalizer to IAsset</td>
    </tr>
    <tr>
      <td><a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/StringLocalizerFactoryAsset.cs">StringLocalizerFactoryAsset</a></td>
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
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/ResourceManagerStringLocalizerAssetSource.cs">ResourceManagerStringLocalizerAssetSource</a></td>
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
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/AssetCachePartStrings.cs">AssetCachePartStrings</a></td>
      <td>A part that adds feature to cache string requests.</td>
    </tr>
    <tr>
      <td>&nbsp;&nbsp;&nbsp;&nbsp;<a href="https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/AssetCachePartCultures.cs">AssetCachePartCultures</a></td>
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
</tbody>
</table>

</details>


# Asset Composition

**AssetComposition** is the default class. It unifies a group of assets into one asset, typically so that they can be assigned to ILineRoot.

```csharp
// Create individual assets
IAsset asset_1 = new StringAsset(new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } }, LineFormat.Parameters);
IAsset asset_2 = new ResourceStringDictionary(new Dictionary<string, byte[]> { { "Culture:en:Key:Hello.Icon", new byte[] { 1, 2, 3 } } }, LineFormat.Parameters);

// Create composition asset
IAssetComposition asset_composition = new AssetComposition(asset_1, asset_2);

// Assign the composition to root
ILineRoot root = new LineRoot(asset_composition, new CulturePolicy());
```

<details>
  <summary><b>IAssetComposition</b> is the interface for classes that composes IAsset components. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Composition of <see cref="IAsset"/> components.
/// </summary>
public interface IAssetComposition : IAsset, IList<IAsset>
{
    /// <summary>
    /// Set to new content.
    /// </summary>
    /// <param name="newContent"></param>
    /// <exception cref="InvalidOperationException">If compostion is readonly</exception>
    void CopyFrom(IEnumerable<IAsset> newContent);

    /// <summary>
    /// Get component assets that implement T. 
    /// </summary>
    /// <param name="recursive">if true, visits children recursively</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>enumerable or null</returns>
    IEnumerable<T> GetComponents<T>(bool recursive) where T : IAsset;
}
```
</details>


# Asset Builder
**AssetBuilder** is a factory class that constructs new instances of IAsset. 
Asset builder is populated with IAssetSources which participate to the build process. 

This example shows how to create asset builder, add asset sources, and then to build an asset.

```csharp
// Create dictionary of strings
Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };

// Create IAssetSource that adds cache 
IAssetSource assetSource_0 = new AssetCacheSource(c => c.AddResourceCache().AddStringsCache().AddCulturesCache());
// Create IAssetSource that static reference of IAsset (string dictionary)
IAssetSource assetSource_1 = new AssetInstanceSource(new StringAsset(strings, LineParameterPrinter.Default) );

// Create AssetBuilder
IAssetBuilder builder = new AssetBuilder(assetSource_0, assetSource_1);
// Instantiate IAsset
IAsset asset = builder.Build();

// Create string key
ILine key = new LineRoot().Key("hello").Culture("en");
// Request value
IString value = asset.GetLine( key ).GetString();
// Print result
Console.WriteLine(value);
```

There are extension methods for convenience.

```csharp
// Create AssetBuilder
IAssetBuilder builder = new AssetBuilder();
// Add IAssetSource that adds cache 
builder.AddCache();
// Add IAssetSource that adds strings
builder.AddStrings(strings, LineParameterPrinter.Default);
```

Asset builders and asset sources are used with Dependency Injection. 
Asset sources are added to IServiceCollection to participate in constructing in new assets.
Asset builder makes the new asset when requested by ServiceProvider.
The calling assembly must have nuget dependency to [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/).


```csharp
// Initialize service collection
IServiceCollection serviceCollection = new ServiceCollection();

// Add IAssetBuilder, an instruction to construct one later
serviceCollection.AddSingleton<IAssetBuilder, AssetBuilder>();
// Add IAssetSource, that will construct cache cache
serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(o => o.AddResourceCache().AddStringsCache().AddCulturesCache()));
// Add IAssetSource, that adds strings
Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
serviceCollection.AddSingleton<IAssetSource>(new AssetInstanceSource(new StringAsset(strings, LineParameterPrinter.Default)));

// Add delegate to forward IAsset request to IAssetBuilder
serviceCollection.AddSingleton<IAsset>(s => s.GetService<IAssetBuilder>().Build());

// Create service scope
using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
{
    // Construct new asset
    IAsset asset = serviceScope.GetService<IAsset>();

    // Create string key
    ILine key = new LineRoot().Key("hello").Culture("en");
    // Request string
    IString value = asset.GetLine(key).GetString();
    // Print result
    Console.WriteLine(value);
}
```

Extension method **AddLexicalLocalization()** adds IAsset, ILineRoot, ICultureProvider and IAssetBuilder services to IServiceCollection.

```csharp
// Initialize service collection
IServiceCollection serviceCollection = new ServiceCollection();

// Add IAssetBuilder, ILineRoot and ICulturePolicy to service collection
serviceCollection.AddLexicalLocalization(
    addStringLocalizerService: false, 
    addCulturePolicyService: false, 
    useGlobalInstance: false,
    addCache: true);

// Add dictionary of strings
Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
serviceCollection.AddSingleton<IAssetSource>(new AssetInstanceSource(new StringAsset(strings, LineParameterPrinter.Default)));

// Create service scope
using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
{
    // Construct new asset
    IAsset asset = serviceScope.GetService<IAsset>();

    // Create string key
    ILine key = new LineRoot().Key("hello").Culture("en");
    // Request string
    IString value = asset.GetLine(key).GetString();
    // Print result
    Console.WriteLine(value);
}
```

<details>
  <summary><b>IAssetBuilder</b> is the interface for factory class(es) that instantiate IAssets. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Builder that can create <see cref="IAsset"/> instance(s).
/// 
/// For dependency injection.
/// </summary>
public interface IAssetBuilder
{
    /// <summary>
    /// List of asset sources that can construct assets.
    /// </summary>
    IList<IAssetSource> Sources { get; }

    /// <summary>
    /// Build language strings.
    /// </summary>
    /// <returns></returns>
    IAsset Build();
}
```
</details>
<details>
  <summary><b>IAssetSource</b> is the interface for sources that contribute asset(s) to the built result. (<u>Click here</u>)</summary>

```csharp
/// <summary>
/// Source of assets. Adds resources to builder's list.
/// </summary>
public interface IAssetSource
{
    /// <summary>
    /// Source adds its <see cref="IAsset"/>s to list.
    /// </summary>
    /// <param name="list">list to add provider(s) to</param>
    /// <returns>self</returns>
    void Build(IList<IAsset> list);

    /// <summary>
    /// Allows source to do post build action and to decorate already built asset.
    /// 
    /// This allows a source to provide decoration such as cache.
    /// </summary>
    /// <param name="asset"></param>
    /// <returns>asset or component</returns>
    IAsset PostBuild(IAsset asset);
}
```
</details>



# Asset Cache
**AssetCache**
is used for caching the requests in cases where asset implementation needs better performance.
Asset cache works as a decorator layer that forwards requests to its *source* and then stores the results. 

Asset cache needs to be populated with [IAssetCacheParts](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs) which each handle caching for a specific interface.

```csharp
// Create asset
var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
IAsset asset = new StringAsset(source, LineFormat.Parameters);

// Create cache
IAssetCache asset_cached = new AssetCache(asset);
// Adds feature to cache IResourceAsset specific requests
asset_cached.Add(new AssetCachePartResources(asset_cached.Source, asset_cached.Options));
// Adds feature to cache IStringAsset specific requests
asset_cached.Add(new AssetCachePartStrings(asset_cached.Source, asset_cached.Options));
// Adds feature to cache IAssetCultureEnumerable specific requests
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

## Caching options
**AssetCacheOptions** carries a key-value map of caching parameters.

```csharp
// Create asset
var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
IAsset asset = new StringAsset(source, LineFormat.Parameters);

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
| CacheStreams | .SetCacheStreams(bool) | true | Should IResourceAsset#OpenStream requests be cached. |
| MaxResourceCount | .SetMaxResourceCount(int) | 2147483647 | Maximum number of resources to cache. |
| MaxResourceSize | .SetMaxResourceSize(int) | 4096 | Maximum size of a resource. |
| MaxResourceTotalSize | .SetMaxResourceTotalSize(int) | 1048576 | Maximum total number of bytes to reserve for all cached resources. |

<br/>
> [!NOTE]
> It is implementation specific whether option is supported or not. Some cache options may not be used.

## Clearing Cache
**IAsset.Reload()** clears caches and reloads assets from their configured sources.


```csharp
// Create asset
var source = new Dictionary<string, string> { { "Culture:en:Key:hello", "Hello World!" } };
IAsset asset = new StringAsset(source, LineFormat.Parameters);

// Cache it
asset = asset.CreateCache();

// Issue a request which will be cached.
ILine key = new LineRoot().Key("hello");
IString value = asset.GetLine( key.Culture("en") ).GetString();
Console.WriteLine(value);

// Clear cache
asset.Reload();
```

# Links
* IAssetComposition
 * [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetComposition)
 * [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
  * [IAssetComposition](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetComposition.cs)
 * [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
  * [AssetComposition](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetComposition.cs)
* IAssetBuilder
 * [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetBuilder)
 * [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
  * [IAssetBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetBuilder.cs)
  * [IAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetSource.cs)
 * [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
  * [AssetBuilder](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetBuilder.cs)
  * [AssetSource](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetSource.cs) Passes IAsset to to builder.
  * [AssetCacheSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCache.cs) Adds cache to the built asset. 
 * [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
  * [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/ResourceManagerStringLocalizerAssetSource.cs) Adapts location of .resources file to IAssetSource.
* IAssetCache
 * [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetCache)
 * [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
  * [IAssetCache](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetCache.cs)
 * [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
  * [AssetCache](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetCache.cs)
  * [AssetCachePartResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCachePartResources.cs)
 * [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/)) 
  * [AssetCachePartStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/AssetCachePartStrings.cs)
  * [AssetCachePartCultures](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/StringAsset/AssetCachePartCultures.cs)
