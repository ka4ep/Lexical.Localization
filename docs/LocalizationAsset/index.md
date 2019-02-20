# Localization Asset
**LocalizationAsset** is simple language string container. Asset is populated from different IEnumeration sources, which become effective content is built with **Load()** method.

**.AddStringSource(*IEnumerable&lt;KeyValuePair&lt;string, string &gt;, string&gt;*)** adds language strings with strings as keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

**.AddKeySource(*IEnumerable&lt;KeyValuePair&lt;Key, string &gt;, string&gt;*)** adds language strings with context-free keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

**.AddAssetKeySource(*IEnumerable&lt;KeyValuePair&lt;IAssetKey, string &gt;, string&gt;*)** adds language strings with IAssetKeys as keys. These keys are converted to Key internally when **.Load()** is called.
[!code-csharp[Snippet](Examples.cs#Snippet_1c)]

Keys can be enumerated with **GetAllKeys()**. 
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

The query can be filtered with a criteria key. It returns only keys that have equal parameters as the criteria key.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]
