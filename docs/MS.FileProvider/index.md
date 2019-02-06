# Ms.FileProvider
*IFileProvider* is an abstaction for the variance of file sources.

Language string files and other localization resources can be read from IFileProvider source.
Loading requires an asset loader and 
[loader part](../IAssetLoader/PartClasses/index.html#file-provider). Note that, the calling assembly must:
* import nuget **Microsoft.Extensions.FileProviders.Abstractions**
* import namespace **Lexical.Localization.Ms.Extensions** 
* import namespace **Lexical.Localization.Ms.Extensions**

# [Snippet](#tab/snippet-1)
[!code-csharp[Snippet](../IAssetLoader/PartClasses/Example_8.cs#Snippet)]
# [Full Code](#tab/full-1)
[!code-csharp[Full Code](../IAssetLoader/PartClasses/Example_8.cs)]
***

<br/>
Asset loader part can also be constructed with 
[Part builder](../IAssetLoader/PartBuilder/index.html#source-type).

# [Snippet](#tab/snippet-2)
[!code-csharp[Snippet](Examples.cs#Snippet)]
# [Full Code](#tab/full-2)
[!code-csharp[Full Code](Examples.cs)]
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
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetLoaderPartBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartBuilder.cs)
 * [AssetLoaderPartFileProviderResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs)
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetLoaderPartBuilder](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartBuilder.cs)
 * [AssetLoaderPartFileProviderStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs)
