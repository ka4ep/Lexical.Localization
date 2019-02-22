# Asset Name Pattern
**AssetNamePattern** is another way of formulating *IAssetKeyNamePolicy*.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

Name pattern consists of parameters. They are written in format of "{prefix **parametername** suffix}".  
Parameter is optional when it's written inside braces "{parameter/}" and required with written inside brackets "[parameter/]".
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

Parameter can be added multiple times by adding suffix "_#". Replace # with the occurance index. "_n" represents the last occurance.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}".
Expressions give more control when name pattern is used for matching against filenames or key-value lines.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

<details>
  <summary><b>IAssetNamePattern</b> is the interface for name patterns. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs#IAssetNamePattern)]
</details>

## Parameters
Some of the parameters are well-known, and they also have a respective method that parametrizes a key.

| Parameter | Key Method  | Description |
|----------|:--------|:------------|
| assembly | .AssemblySection(*string*) | Assembly name |
| location | .Location(*string*) | Subdirectory in local files |
| resource | .Resource(*string*) | Subdirectory in embedded resources |
| type | .TypeSection(*string*) | Class name |
| section | .Section(*string*) | Generic section, used for grouping |
| anysection | *all above* | Matches to any section above. |
| culture  | .SetCulture(*string*) | Culture |
| key | .Key(*string*) | Key name |

<br/>
Custom parameters can be created. Parameter key object should implement IAssetKey, have [AssetKeyParameter] attribute and [AssetKeyConstructor] in the method that creates it.


# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 