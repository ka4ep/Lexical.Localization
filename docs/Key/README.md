# Key
**Key** is context free format of a localization key. Essentially it is an array of keys and values. 
*Key* is used as an internal class for serializing localization files and for comparing context-dependent keys.

```csharp
Key key = new Key("Culture", "en").Append("Type", "MyController").Append("Key", "Success");
```

Key can be converted to context dependent key IAssetKey with a parametrizer.

```csharp

```

# Links
 * [Key](https://github.com/tagcode/Lexical.Localization/blob/master/Lexical.Localization/Internal/Key.cs) is context-free key format.
