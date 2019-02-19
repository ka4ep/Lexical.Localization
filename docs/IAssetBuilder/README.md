# Asset Builder
**AssetBuilder** is a factory class that constructs new instances of IAsset. 
Asset builder is populated with IAssetSources which participate to the build process. 

This example shows how to create asset builder, add asset sources, and then to build an asset.

```csharp
// Create dictionary of strings
Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };

// Create IAssetSource that adds cache 
IAssetSource assetSource_0 = new AssetCacheSource(c => c.AddResourceCache().AddStringsCache().AddKeysCache().AddCulturesCache());
// Create IAssetSource that static reference of IAsset (string dictionary)
IAssetSource assetSource_1 = new AssetSource(new LocalizationStringDictionary(strings, AssetKeyNameProvider.Default) );

// Create AssetBuilder
IAssetBuilder builder = new AssetBuilder(assetSource_0, assetSource_1);
// Instantiate IAsset
IAsset asset = builder.Build();

// Create string key
IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
// Request string
string str = asset.GetString( key );
// Print result
Console.WriteLine(str);
```

There are extension methods for convenience.

```csharp
// Create AssetBuilder
IAssetBuilder builder = new AssetBuilder();
// Add IAssetSource that adds cache 
builder.AddCache();
// Add IAssetSource that adds strings
builder.AddDictionary(strings, AssetKeyNameProvider.Default);
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
serviceCollection.AddSingleton<IAssetSource>(new AssetCacheSource(o => o.AddResourceCache().AddStringsCache().AddKeysCache().AddCulturesCache()));
// Add IAssetSource, that adds strings
Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
serviceCollection.AddSingleton<IAssetSource>(new AssetSource(new LocalizationStringDictionary(strings, AssetKeyNameProvider.Default)));

// Add delegate to forward IAsset request to IAssetBuilder
serviceCollection.AddSingleton<IAsset>(s => s.GetService<IAssetBuilder>().Build());

// Create service scope
using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
{
    // Construct new asset
    IAsset asset = serviceScope.GetService<IAsset>();

    // Create string key
    IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
    // Request string
    string str = asset.GetString(key);
    // Print result
    Console.WriteLine(str);
}
```

Extension method **AddLexicalLocalization()** adds IAsset, IAssetRoot, ICultureProvider and IAssetBuilder services to IServiceCollection.

```csharp
// Initialize service collection
IServiceCollection serviceCollection = new ServiceCollection();

// Add IAssetBuilder, IAssetRoot and ICulturePolicy to service collection
serviceCollection.AddLexicalLocalization(
    addStringLocalizerService: false, 
    addCulturePolicyService: false, 
    useGlobalInstance: false,
    addCache: true);

// Add dictionary of strings
Dictionary<string, string> strings = new Dictionary<string, string> { { "en:hello", "Hello World!" } };
serviceCollection.AddSingleton<IAssetSource>(new AssetSource(new LocalizationStringDictionary(strings, AssetKeyNameProvider.Default)));

// Create service scope
using (ServiceProvider serviceScope = serviceCollection.BuildServiceProvider())
{
    // Construct new asset
    IAsset asset = serviceScope.GetService<IAsset>();

    // Create string key
    IAssetKey key = new LocalizationRoot().Key("hello").SetCulture("en");
    // Request string
    string str = asset.GetString(key);
    // Print result
    Console.WriteLine(str);
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

# Links
* [Example code](https://github.com/tagcode/Lexical.Localization/tree/master/docs/IAssetBuilder)
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetBuilder.cs)
 * [IAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/Asset/IAssetSource.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetBuilder](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetBuilder.cs)
 * [AssetSource](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization/Asset/AssetSource.cs) Passes IAsset to to builder.
 * [AssetCacheSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Asset/AssetCache.cs) Adds cache to the built asset. 
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [ConfigurationBuilderLocalizationAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Configuration/ConfigurationBuilderLocalizationAssetSource.cs) Adapts IConfigurationBuilder to IAssetSource.
 * [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/Localization/ResourceManagerStringLocalizerAssetSource.cs) Adapts location of .resources file to IAssetSource.
