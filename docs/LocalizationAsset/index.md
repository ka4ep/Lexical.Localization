# Localization Asset
**LocalizationAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.

**.AddKeySource(*IEnumerable&lt;KeyValuePair&lt;Key, string&gt;, string&gt;*)** add a source of language strings.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

Language strings can now be queried from the asset.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

<details>
  <summary><b>.AddStringSource()</b> adds language string source with String based keys. (<u>Click here</u>)</summary>
These keys are converted to Key internally when <b>.Load()</b> is called.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
</details>

<details>
  <summary><b>.AddAssetKeySource()</b> adds language string source with IAssetKey based keys. (<u>Click here</u>)</summary>
These keys are converted to Key internally when <b>.Load()</b> is called.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]
</details>
<br/>

Keys can be enumerated with **GetAllKeys()**. 
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]

The query can be filtered with a criteria key. It returns only keys that have equal parameters as the criteria key.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]
