# Asset Key Parametrizer
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
[!code-csharp[Snippet](../../Lexical.Localization.Abstractions/AssetKey/IAssetNamePattern.cs#IAssetNamePattern)]
</details>

# Asset Key Parametrizer
The context free format of a key is an array of keys and values **IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;**.

**AssetKeyParametrizer** converts implementations of *IAssetKey* to *IEnumerable*
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

And back to IAssetKey.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]
 