# Introduction
Parts are added to asset loader. They load IAsset instances on need basis.
[!code-csharp[Snippet](Example_15.cs#Snippet)]

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
[!code-csharp[Snippet](Example_0.cs#Snippet)]

# Embedded Strings
In similar way to loading assets from files, they can also be loaded from embedded resources. Let's assume your application has the following embedded resources.
```none
docs.Assets.localization.ini
docs.Assets.localization-sv.ini
docs.Assets.localization-fi.ini
docs.Assets.localization-de.ini
```

Asset loader is configured in the same manner, but with different class.
[!code-csharp[Snippet](Example_2.cs#Snippet)]

# Binary resources
There are two types of localization assets: strings and binary resources.
Binary resources are graphics, icons, audio files, and other resource that are language and location specific.
The asset part classes that load binaries have slightly different naming, 
the is distinction in the end of the class names: **AssetLoaderPartFile*Strings*** loads strings and **AssetLoaderPartFile*Resource*** loads binaries.

**AssetLoaderPartFileResources** loads binary resources from files.
[!code-csharp[Snippet](Example_4.cs#Snippet)]

**AssetLoaderPartEmbeddedResources** loads binary resources from *embedded resources*.
[!code-csharp[Snippet](Example_6.cs#Snippet)]

# File Provider
Third type of source is [IFileProvider](https://github.com/aspnet/Extensions/blob/master/src/FileProviders/Abstractions/src/IFileProvider.cs).
[AssetLoaderPartFileProviderStrings](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderStrings.cs)
loads strings from file providers. The calling assembly must have nuget dependency to **[Microsoft.Extensions.FileProviders.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.FileProviders.Abstractions/)**,
and import namespaces **Lexical.Localization.Ms.Extensions** and **Lexical.Localization.Ms.Extensions**.
# [Snippet](#tab/snippet-8)
[!code-csharp[Snippet](Example_8.cs#Snippet)]
# [Full Code](#tab/full-8)
[!code-csharp[Snippet](Example_8.cs)]
***


And [AssetLoaderPartFileProviderResources](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Ms.Extensions/FileProvider/AssetLoaderPartFileProviderResources.cs)
loads binary resources.

# [Snippet](#tab/snippet-10)
[!code-csharp[Snippet](Example_10.cs#Snippet)]
# [Full Code](#tab/full-10)
[!code-csharp[Snippet](Example_10.cs)]
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
[!code-csharp[Snippet](Example_14.cs#Snippet)]

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
