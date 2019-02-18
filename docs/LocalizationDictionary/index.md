# Localization String Dictionary
**LocalizationDictionary** is the simplest asset. It uses a dictionary as a source.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

If LocalizationDictionary was constructed with a [name pattern](../IAssetKeyNamePolicy/index.html#asset-name-pattern) (IAssetNamePattern or string) then keys can be enumerated from it.
[!code-csharp[Snippet](Examples.cs#Snippet_2)]

**GetAllKeys()** can be filtered with a criteria key.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]
