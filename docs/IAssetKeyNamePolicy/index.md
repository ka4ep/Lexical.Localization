# Asset Key Name Policy
Asset key name policy is a mechanism that converts IAssetKeys into identity strings so that they can be match against lines in localization sources.

For instance, if localization source has separator character '/', 
then the loading asset must be instructed to use a policy that matches the separator. 
[!code-csharp[Snippet](Examples.cs#Snippet_0a)]

Extension method **.Build(*IAssetKey*)** can be used to test the conversion from key to identity. It forwards the method call to correct sub-interface.
[!code-csharp[Snippet](Examples.cs#Snippet_0b)]

<details>
  <summary><b>IAssetKeyNamePolicy</b> is the root interface for classes that formulate **IAssetKey**s into identity strings. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/AssetKey/IAssetKeyNamePolicy.cs#IAssetKeyNamePolicy)]
</details>

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
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/AssetKey/IAssetKeyNamePolicy.cs#IAssetKeyNameProvider)]
</details>

# Asset Name Pattern
**AssetNamePattern** is another way of formulating *IAssetKeyNamePolicy*.
[!code-csharp[Snippet](Examples.cs#Snippet_4)]

Name pattern consists of parameters. They are written in format of "{prefix **parametername** suffix}".  
Parameter is optional when it's written inside braces "{parameter/}" and required with written inside brackets "[parameter/]".
[!code-csharp[Snippet](Examples.cs#Snippet_4a)]

Parameter can be added multiple times by adding suffix "_#". Replace # with the occurance index. "_n" represents the last occurance.
[!code-csharp[Snippet](Examples.cs#Snippet_4b)]

Regular expression can be written inside angle brackets "{parameter&lt;*regexp*&gt;/}".
Expressions give more control when name pattern is used for matching against filenames or key-value lines.
[!code-csharp[Snippet](Examples.cs#Snippet_4c)]

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

<details>
  <summary><b>IAssetNamePattern</b> is the interface for name patterns. (<u>Click here</u>)</summary>
[!code-csharp[Snippet](../../Lexical.Localization/Abstractions/AssetKey/IAssetNamePattern.cs#IAssetNamePattern)]
</details>

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [IAssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is a subinterface where Build() can be implemented directly.
 * [IAssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Abstractions/AssetKey/IAssetNamePattern.cs) is a subinterface that formulates parametrization with a template string.
* [Lexical.Localization](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization) ([NuGet](https://www.nuget.org/packages/Lexical.Localization/))
 * [AssetKeyNameProvider](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/AssetKey/AssetKeyNameProvider.cs) is implementation of IAssetNameProvider.
 * [AssetNamePattern](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Localization/AssetKey/AssetNamePattern.cs) is the default implementation of IAssetNamePattern.
 