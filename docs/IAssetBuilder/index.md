# Asset Builder
**AssetBuilder** is a factory class that constructs new instances of IAsset. 
Asset builder is populated with IAssetSources which participate to the build process. 

This example shows how to create asset builder, add asset sources, and then to build an asset.
[!code-csharp[Snippet](Example_0.cs#Snippet)]

There are extension methods for convenience.
[!code-csharp[Snippet](Example_1.cs#Snippet)]

Asset builders and asset sources are used with Dependency Injection. 
Asset sources are added to IServiceCollection to participate in constructing in new assets.
Asset builder makes the new asset when requested by ServiceProvider.
The calling assembly must have nuget dependency to [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/).

[!code-csharp[Snippet](Example_2.cs#Snippet)]

Extension method **AddLexicalLocalization()** adds IAsset, IAssetRoot, ICultureProvider and IAssetBuilder services to IServiceCollection.
[!code-csharp[Snippet](Example_3.cs#Snippet)]

<details>
  <summary><b>IAssetBuilder</b> is the interface for factory class(es) that instantiate IAssets. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetBuilder.cs#interface)]
</details>
<details>
  <summary><b>IAssetSource</b> is the interface for sources that contribute asset(s) to the built result. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/Asset/IAssetSource.cs#interface)]
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
 * [ResourceManagerStringLocalizerAssetSource](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/LocalizationAsset/ResourceManagerStringLocalizerAssetSource.cs) Adapts location of .resources file to IAssetSource.
