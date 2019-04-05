# Localization Asset
**LocalizationAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective when **Load()** is called.

<b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;string, string&gt;&gt;, IAssetNamePolicy keyPolicy</i>)</b> adds language string source with String based keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]
<br/>

<b>.Add(<i>IEnumerable&lt;KeyValuePair&lt;IAssetKey, string&gt;&gt;</i>, IAssetNamePolicy keyPolicy)</b> adds language string source with IAssetKey based keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]
<br/>

Keys can be enumerated with **GetAllKeys()**. 
[!code-csharp[Snippet](Examples.cs#Snippet_3a)]
<br/>

The query can be filtered with a criteria key. It returns only keys that have equal parameters as the criteria key.
[!code-csharp[Snippet](Examples.cs#Snippet_3b)]
