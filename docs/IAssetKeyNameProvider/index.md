# Asset Key Name Provider
**AssetKeyNameProvider** is a class that appends key parts together. 
It starts with non-canonical parts, and then canoncial parts.

Let's create an example key.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]
And now, let's try out different policies to see how they look.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

Custom policies can be created by instantiating AssetKeyNameProvider and adding configurations.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

<details>
  <summary><b>IAssetKeyNameProvider</b> is policy interface where Build() can be implemented directly. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs#IAssetKeyNameProvider)]
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 