# Parameter Name Policy
**ParameterNamePolicy** is an *IAssetNameKeyPolicy* implementation that uses context free string format. 
Keys that are written in this format do not need contextual information about the notation of each line as the notatin contains both parameter name and value.

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
IAssetKey key = ParameterNamePolicy.Instance.Parse(str, LocalizationRoot.Global);
```

# Links
* [Lexical.Localization.Abstractions](https://github.com/tagcode/Lexical.Localization/tree/master/Lexical.Localization.Abstractions) ([NuGet](https://www.nuget.org/packages/Lexical.Localization.Abstractions/))
 * [IAssetKeyNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/IAssetKeyNamePolicy.cs) is the root interface for classes that formulate IAssetKey into identity string.
 * [ParameterNamePolicy](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization.Abstractions/AssetKey/Key.NamePolicy.cs) is context-free string format.
