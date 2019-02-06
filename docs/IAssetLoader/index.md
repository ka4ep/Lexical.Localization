# Introduction
**AssetLoader** is a class that loads assets on-need-basis. 
It must be populated with parts, either with [part builder helper](PartBuilder/index.md) or by [constructing from classes](PartClasses/index.md). 

[!code-csharp[Snippet](Example_12.cs#Snippet)]

<p/>
<details>
  <summary><b>IAssetProvider</b> is interface that is intended for consumers. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/Asset/IAssetProvider.cs#interface)]
</details>
<details>
  <summary><b>IAssetLoader</b> is interface for the initializers. Loader parts are added here. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/Asset/IAssetLoader.cs#interface)]
</details>
<details>
  <summary><b>IAssetLoaderPart</b> is the interface for classes that participate in the asset loading process. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/Asset/IAssetLoaderPart.cs#interface)]
</details>
<details>
  <summary><b>IAssetLoaderPartOptions</b> is key-value dictionary interface for options. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/Asset/IAssetLoaderPartOptions.cs#interface)]
</details>

# Links
* [Lexical.Asset.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Asset.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Asset.Abstractions/))
 * [IAssetLoader](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetLoader.cs)
 * [IAssetLoaderPart](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetLoaderPart.cs)
 * [IAssetLoaderPartOptions](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/Asset/IAssetLoaderPartOptions.cs)
* [Lexical.Asset](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Asset) ([NuGet](https://www.nuget.org/packages/Lexical.Asset/))
 * [AssetLoader](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/AssetLoader/AssetLoader.cs) is the default implementation of IAssetLoader.
