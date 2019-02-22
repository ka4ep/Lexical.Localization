# Asset Key Name Policy
Asset key name policy is a pattern rule mechanism that converts IAssetKeys into identity strings so that they can matched against lines in identity based localization sources.

For instance, if localization source has separator character '/', 
then the loading asset must be instructed to use a policy that matches the separator. 
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]

Extension method **.Build(*IAssetKey*)** can be used to test the conversion from key to identity. It forwards the method call to correct sub-interface.
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

<details>
  <summary><b>IAssetKeyNamePolicy</b> is the root interface for classes that formulate **IAssetKey**s into identity strings. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs#IAssetKeyNamePolicy)]
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
 * [Key.NamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.NamePolicy.cs) is context-free key format.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 