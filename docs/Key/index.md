# Key
**Key** is context free format of a localization key. Essentially it is an array of keys and values. 
*Key* is used as an internal class for serializing localization files and for comparing context-dependent keys.
[!code-csharp[Snippet](Examples.cs#Snippet_1)]

Key can be converted to context dependent key IAssetKey with a parametrizer.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

# Key Name Policy
**Key.NamePolicy** is an *IAssetNameKeyPolicy* implementation that uses string format which contains full parameter names and values. This makes it usable without foreknowledge of line notation.

```none
parameterName:parameterValue:parameterName:parameterValue:...
```

For example:
```none
culture:en:type:MyController:key:Success
```

It uses the following escape rules.

| Sequence | Meaning |
|:---------|:--------|
| \\: | Colon |
| \\t | Tab |
| \\r | Carriage return |
| \\n | New line |
| \\xnnnn | Unicode 16bit surrogate |
| \\unnnn | Unicode variable length surrogate |

For example to escape key "Success:Plural" would be
```none
key:Success\:Plural
```

**Key.NamePolicy** prints IAssetKey as string that contains parameter names and values.
[!code-csharp[Snippet](Examples.cs#Snippet_5)]

And parses them back to IAssetKey.
[!code-csharp[Snippet](Examples.cs#Snippet_6)]

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [Key](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.cs) is context-free key format.
 * [Key.NamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.NamePolicy.cs) is context-free string format.
