# Key
**Key** is context free format of a localization key. Essentially it is an array of keys and values. 
*Key* is used as an internal class for serializing localization files and for comparing context-dependent keys.

```csharp
Key key = new Key("culture", "en").Append("type", "MyController").Append("key", "Success");
```

Key can be converted to context dependent key IAssetKey with a parametrizer.

```csharp
// Create context-dependent key
IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
// Serialize to string
string str = ParameterNamePolicy.Instance.PrintKey(key);
```

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

```csharp
// Create context-dependent key
IAssetKey key = LocalizationRoot.Global.TypeSection("MyController").Key("Success").SetCulture("en");
// Serialize to string
string str = ParameterNamePolicy.Instance.PrintKey(key);
```

And parses them back to IAssetKey.

```csharp
// Key in string format
string str = "culture:en:type:MyLibrary.MyController:key:Success";
// Parse string
IAssetKey key = ParameterNamePolicy.Instance.ParseKey(str, LocalizationRoot.Global);
```

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [Key](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.cs) is context-free key format.
 * [Key.NamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.NamePolicy.cs) is context-free string format.
