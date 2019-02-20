# Localization String Dictionary
**LocalizationAsset** is simple language string container. It uses Dictionary&lt;string, string&gt; as a source.

When a language string is requested the requested IAssetKey is converted to string to be matched with the dictionary. 
[IAssetKeyNamePolicy](../IAssetKeyNamePolicy/index.html) is used for making the conversion.
[!code-csharp[Snippet](Examples.cs#Snippet_1a)]

If the provided key name policy is *AssetKeyNameProvider.Default*, then identity string is constructed by concatenating parameters from requesting key in the order of appearance. Non-canonical parameter "culture" is, however, appended first.
[!code-csharp[Snippet](Examples.cs#Snippet_1b)]

If LocalizationStringAsset was constructed with a [IAssetNamePattern](../IAssetKeyNamePolicy/index.html#asset-name-pattern) (or string), then parameters from IAssetKey substituted to the parameter names in the pattern.
[!code-csharp[Snippet](Examples.cs#Snippet_2a)]

Also keys can be enumerated from the asset.
[!code-csharp[Snippet](Examples.cs#Snippet_2b)]

**GetAllKeys()** can be filtered with a criteria key.
[!code-csharp[Snippet](Examples.cs#Snippet_3)]
