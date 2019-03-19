# Asset Name Pattern
**AssetNamePattern** is another way of formulating *IAssetKeyNamePolicy*.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

Name pattern consists of parameters. They are written in format of "{prefix **parametername** suffix}".  
Parameter is optional when it's written inside braces "{parameter/}" and required with written inside brackets "[parameter/]".
[!code-csharp[Snippet](Examples.cs#Snippet_3)]

Parameter can be added multiple times.
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]

A shorter way to add conscutive parameters is use suffix "_n". It translates to the five occurance.
If part is required, e.g. "[parametername_n]", then only first part is required and others are optional.
[!code-csharp[Snippet](Examples.cs#Snippet_4c)]

Parameters need to be added in non-consecutive order, then a suffix "_#" which represents the occurance index.
[!code-csharp[Snippet](Examples.cs#Snippet_4d)]

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
| Assembly | .Assembly(*string*) | Assembly name |
| Location | .Location(*string*) | Subdirectory in local files |
| Resource | .Resource(*string*) | Subdirectory in embedded resources |
| Type | .Type(*string*) | Class name |
| Section | .Section(*string*) | Generic section, used for grouping |
| anysection | *all above* | Matches to any section above. |
| Culture  | .Culture(*string*) | Culture |
| Key | .Key(*string*) | Key name |
| N | .N(*Type*) | Plurality key |

<br/>
Custom parameters can be created. Parameter key object should implement IAssetKey, have [AssetKeyParameter] attribute and [AssetKeyConstructor] in the method that creates it.


# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 